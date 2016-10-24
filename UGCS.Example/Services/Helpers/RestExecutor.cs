using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class RestExecutor
    {

        /// <summary>
        /// Response handler for the requests.
        /// </summary>
        /// <param name="response">Deserialized response object. Null if no response 
        /// type was provided, just indicates successfull HTTP response.</param>
        /// <param name="error">Exception if any occurred. Null if no error.</param>
        public delegate void ResponseHandler(object response, Exception error);

        /// <summary>
        /// Dummy type to indicate that response processing is not required.
        /// </summary>
        public class NullResponse { }

        /// <summary>
        /// String used as user-agent ID in the requests.
        /// </summary>
        public string userAgent = "U[g]CS";

        /// <summary>
        /// Request to the server.
        /// </summary>
        public class Request
        {
            /// <summary>
            /// Request path. Leading slash is not needed.
            /// </summary>
            public string request;
            /// <summary>
            /// Method to use for HTTP request.
            /// </summary>
            public string method = "GET";
            /// <summary>
            /// Type for response object. typeof(NullResponse) if response body is not
            /// needed. Null to get Stream object as the result. Otherwise should be
            /// JSON-serializable type.
            /// </summary>
            public Type responseType = null;
            /// <summary>
            /// Body for request if needed. Should be either JSON-serializable object,
            /// byte[] or Stream.
            /// </summary>
            public object body = null;
            /// <summary>
            /// MIME type if body is byte[].
            /// </summary>
            public string bodyMimeType = null;
            /// <summary>
            /// Type for response object in case of error. Set
            /// typeof(NullResponse) if response body is not needed.
            /// Leave null to get Stream object as the result. Otherwise should be
            /// JSON-serializable type.
            /// </summary>
            public Type errorType = null;

            /// <summary>
            /// Invoked on response in fetcher thread.
            /// </summary>
            public ResponseHandler responseHandler;

            public Request(string request)
            {
                this.request = request;
            }

            /// <summary>
            /// Invoke the response handler with the provided response data.
            /// </summary>
            internal void
            Response(object response)
            {
                if (responseHandler != null)
                {
                    responseHandler(response, null);
                }
            }

            /// <summary>
            /// Invoke the response handler when error occurred.
            /// </summary>
            internal void
            Error(Exception error)
            {
                if (responseHandler != null)
                {
                    responseHandler(null, error);
                }
            }
        }

        /// <summary>
        /// Helper for constructing GET request iwth the desired response type.
        /// </summary>
        /// <typeparam name="TResponse">Desired response type. NullResponse if not needed.
        /// </typeparam>
        public class Request<TResponse> : Request
        {
            public Request(string request) :base(request)
            {
                responseType = typeof(TResponse);
            }
        }

        /// <summary>
        /// Helper for constructing POST request iwth the desired response type.
        /// </summary>
        /// <typeparam name="TResponse">Desired response type.</typeparam>
        public class PostRequest<TResponse> : Request<TResponse>
        {
            public
            PostRequest(string request, object body) :
                base(request)
            {
                method = "POST";
                this.body = body;
            }
        }

        /// <summary>
        /// Create the executor.
        /// </summary>
        /// <param name="host">Target host.</param>
        /// <param name="port">Target port.</param>
        /// <param name="log">Log for diagnostics if necessary.</param>
        public
        RestExecutor(string host, int port = 80, ILog log = null)
        {
            this.log = log;
            this.host = host;
            restPort = port;
            thread = new Thread(FetchThread);
            thread.Start();
        }

        /// <summary>
        /// Send request to the server.
        /// </summary>
        public void
        SendRequest(Request req)
        {
            lock (reqQueue)
            {
                if (isDisposed)
                {
                    req.Error(new Exception("Connection disposed"));
                    return;
                }
                reqQueue.Enqueue(req);
                Monitor.PulseAll(reqQueue);
            }
        }

    
        private string host;
        private int restPort;
        private Thread thread;
        private bool exit = false;
        private Queue<Request> reqQueue = new Queue<Request>();
        private ILog log;
        private bool isDisposed = false;

        private class HttpResponse
        {
            private readonly Stream _responseStream;

            private readonly HttpStatusCode _httpCode;

            public Stream ResponseStream { get { return _responseStream; } }

            public HttpStatusCode HttpCode { get { return _httpCode; } }

            public HttpResponse(Stream responseStream, HttpStatusCode httpCode)
            {
                _responseStream = responseStream;
                _httpCode = httpCode;
            }
        }

        public void
        Dispose()
        {
            lock (reqQueue)
            {
                exit = true;
                isDisposed = true;
                Monitor.PulseAll(reqQueue);
            }
            thread.Join();
            var e = new Exception("Connection disposed");
            foreach (var req in reqQueue)
            {
                req.Error(e);
            }
            reqQueue.Clear();
        }

        private HttpResponse HttpRequest(Uri uri, String method = "GET", Stream body = null, String bodyMimeType = null)
        {
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(uri);
                request.UseDefaultCredentials = true;
                request.Method = method;
                request.UserAgent = userAgent;
                request.Accept = "application/json";
                if (body != null)
                {
                    request.ContentLength = body.Length;
                    request.ContentType = bodyMimeType;
                    request.AllowWriteStreamBuffering = false;
                    using (Stream s = request.GetRequestStream())
                    {
                        byte[] buffer = new byte[0x8000];
                        int read;
                        while ((read = body.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            s.Write(buffer, 0, read);
                        }
                    }
                    Thread.Sleep(1000);
                    body.Close();
                    body = null;
                }

                HttpWebResponse response;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        response = (HttpWebResponse)ex.Response;
                    }
                    else
                    {
                        throw;
                    }
                }

                return new HttpResponse(response.GetResponseStream(), response.StatusCode);
            }
            catch (Exception)
            {
                if (body != null)
                {
                    body.Close();
                }
                throw;
            }
        }

        /// <summary>
        /// Send the request to server.
        /// </summary>
        /// <returns>Response data stream.</returns>
        private HttpResponse  SendHttpRequest(Request req, out Uri uri)
        {
            uri = new Uri(String.Format("http://{0}:{1}/{2}",
                                        host, restPort, req.request));
            Stream body = null;
            String bodyType = null;
            if (req.body != null)
            {
                if (req.body.GetType() == typeof(byte[]))
                {
                    body = new MemoryStream((byte[])req.body);
                    bodyType = req.bodyMimeType;
                }
                else if (typeof(Stream).IsAssignableFrom(req.body.GetType()))
                {
                    body = (Stream)req.body;
                    bodyType = req.bodyMimeType;
                }
                else
                {
                    body = new MemoryStream();
                    var serializer = new DataContractJsonSerializer(req.body.GetType());
                    serializer.WriteObject(body, req.body);
                    body.Position = 0;
                    bodyType = "application/json;charset=utf-8";
                }
            }
            return HttpRequest(uri, req.method, body, bodyType);
        }

        private void FetchThread()
        {
            while (true)
            {
                Request req;
                lock (reqQueue)
                {
                    while (!exit && reqQueue.Count == 0)
                    {
                        Monitor.Wait(reqQueue);
                    }
                    if (exit)
                    {
                        return;
                    }
                    if (reqQueue.Count == 0)
                    {
                        continue;
                    }
                    req = reqQueue.Dequeue();
                }

                HttpResponse response;
                Uri uri = null;
                try
                {
                    response = SendHttpRequest(req, out uri);
                }
                catch (Exception e)
                {
                    if (log != null)
                    {
                        log.WarnFormat("REST request exception for \"{0}\": {1}",
                                       uri == null ? "<null>" : uri.ToString(),
                                       e.ToString());
                    }

                    // FIXME invocations of this type are not safe because may
                    // throw exceptions thus interrupting the fetching thread
                    req.Error(e);
                    continue;
                }

                if (response.HttpCode != HttpStatusCode.OK) // FIXME other codes may be OK too
                {
                    object errorData;
                    try
                    {
                        errorData = _deserializeResponseBody(req.errorType, response.ResponseStream);
                    }
                    catch (Exception e)
                    {
                        if (log != null)
                        {
                            log.WarnFormat("Response parsing exception: {0}\nType: {1}",
                                            e.ToString(), req.responseType.ToString());
                        }
                        req.Error(new HttpRemoteException(response.HttpCode, e));
                        continue;
                    }


                    req.Error(new HttpRemoteException(response.HttpCode, errorData));
                    continue;
                }

                object result;
                try
                {
                    result = _deserializeResponseBody(req.responseType, response.ResponseStream);
                }
                catch (Exception e)
                {
                    if (log != null)
                    {
                        log.WarnFormat("Response parsing exception: {0}\nType: {1}",
                                        e.ToString(), req.responseType.ToString());
                    }
                    req.Error(e);
                    continue;
                }
                req.Response(result);
            }
        }

        private object _deserializeResponseBody(Type type, Stream responseStream)
        {
            if (type != typeof(NullResponse))
            {
                if (type == null)
                {
                    return responseStream;
                }
                else
                {
                    return new DataContractJsonSerializer(type).ReadObject(responseStream);
                }
            }
            return null;
        }


    }
}
