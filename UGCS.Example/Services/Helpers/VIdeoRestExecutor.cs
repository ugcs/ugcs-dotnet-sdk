using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class VideoRestExecutor : IDisposable
    {
        private string host;
        private int port;
        private List<SourceEntry> sources = new List<SourceEntry>();
        private bool refreshInProgress = false;
        private RestExecutor executor;

        public class SourceEntry
        {

            public class Broadcast
            {

                public enum Status
                {
                    /// <summary>
                    /// The broadcast type is not available.
                    /// </summary>
                    NOT_AVAILABLE = 0,
                    DISABLED = 1,
                    STARTING = 2,
                    BROADCASTING = 3,
                    ERROR = 4,
                    /// <summary>
                    /// Set when unknown status received from the server.
                    /// </summary>
                    UNKNOWN
                }

                /// <summary>
                /// Broadcast type.
                /// </summary>
                public string type;
                /// <summary>
                /// Broadcasting URL.
                /// </summary>
                public string url;
                /// <summary>
                /// Current status of the broadcast.
                /// </summary>
                public Status status;
                /// <summary>
                /// Status messages from the server.
                /// </summary>
                public string statusMsg;
                /// <summary>
                /// Error code if Status.Error.
                /// </summary>
                public int errorCode;
            }

            /// <summary>
            /// Source name.
            /// </summary>
            public string name;
            /// <summary>
            /// Source URI.
            /// </summary>
            public string uri;
            /// <summary>
            /// Stream port used as identifier.
            /// </summary>
            public int port;
            /// <summary>
            /// Associated broadcasts.
            /// </summary>
            public List<Broadcast> broadcasts = new List<Broadcast>();

            /// <summary>
            /// A temporary hack to store recording status
            /// </summary>
            public bool isRecording = false;
        }


        /// <summary>
        /// Metadata from the geoserver.
        /// Contains information about a particular video
        /// For now - just length of it
        /// </summary>
        [Serializable]
        public class VideoDurationInfo
        {
            public int duration;
        }


        /// <summary>
        /// Called when new sources list is received.
        /// </summary>
        /// <param name="sources">
        /// Sources list. May be null to indicate connection error.
        /// </param>
        public delegate void RefreshHandler(List<SourceEntry> sources);

        /// <summary>
        /// Fired in Unity thread when list of sources is refreshed.
        /// </summary>
        public event RefreshHandler OnRefresh;

        public VideoRestExecutor(string host, int port)
        {
            this.host = host;
            this.port = port;
            executor = new RestExecutor(host, port, null);
        }

        /// <summary>
        /// Send request to the server. The response handler specified in the request
        /// is called in Unity thread.
        /// </summary>
        public void  SendRequest(RestExecutor.Request req)
        {
            var handler = req.responseHandler;
            if (handler != null)
            {
                req.responseHandler = (object response, Exception error) =>
                {
                    handler(response, error);
                };
            }
            executor.SendRequest(req);
        }

        /// <summary>
        /// Initiate asynchronous refresh request to REST server. OnRefreshed
        /// event is fired once done.
        /// </summary>
        public void Refresh()
        {

            if (refreshInProgress)
            {
                return;
            }
            refreshInProgress = true;
            var req = new RestExecutor.Request<StreamData[]>("streams");
            req.responseHandler = ProcessRefreshResponse;
            SendRequest(req);
        }


        /// <summary>
        /// Initiate recording of specified stream or stops it
        /// </summary>
        public void Record(int streamPort, bool setRecorgingOn, string videoID, RestExecutor.ResponseHandler onResponse)
        {
            var req = new RestExecutor.Request("stream");
            string tmpStr = "{ \"port\": " + streamPort + ", \"is_recording_active\": "
                + setRecorgingOn.ToString().ToLower() + ", \"video_id\": \"" + videoID + "\" }";
            byte[] buffer = Encoding.ASCII.GetBytes(tmpStr);
            req.body = buffer;
            req.bodyMimeType = "application/json; charset=UTF-8";
            req.method = "PUT";
            req.responseHandler = onResponse;
            SendRequest(req);
        }

        /// <summary>
        /// Get video length by video ID
        /// </summary>
        public void GetVideoLength(string videoID, RestExecutor.ResponseHandler onResponse = null)
        {
            var req = new RestExecutor.Request("/video/" + videoID);
            req.method = "GET";
            req.responseHandler = onResponse;
            SendRequest(req);
        }


        /// <summary>
        /// Request video playback by video ID at given speed and from specified position
        /// </summary>
        public string Playback(string videoID, long position = 0, float speed = 1)
        {
            string uriString = String.Format("/playback?video_id={0}&pos={1}&speed={2}",
                videoID, position, speed);

            var builder = new UriBuilder();
            builder.Scheme = "http";
            builder.Host = host;
            builder.Port = port;

            Uri retUri = new Uri(builder.Uri, uriString);
            //UnityEngine.Debug.Log(retUri.ToString());
            return retUri.ToString();
        }

        /// <summary>
        /// Removes video by video ID
        /// </summary>
        public void Remove(string videoID, RestExecutor.ResponseHandler onResponse = null)
        {
            var req = new RestExecutor.Request(
                String.Format("/video/{0}",
                videoID));
            req.method = "DELETE";
            req.responseHandler = onResponse;
            SendRequest(req);
        }

        /// <summary>
        /// Exports video by video ID
        /// </summary>
        public void Export(string videoID, RestExecutor.ResponseHandler onResponse = null)
        {
            var req = new RestExecutor.Request(String.Format("/download/{0}", videoID));
            req.method = "GET";
            req.responseHandler = onResponse;
            SendRequest(req);
        }

        [DataContract]
        protected class StreamBroadcast
        {
            [DataMember]
            public string url;

            [DataMember]
            public int state;

            [DataMember]
            public string state_msg;

            [DataMember]
            public string type;

            [DataMember]
            public int error_code = 0;
        }

        [DataContract]
        protected class StreamData
        {
            [DataMember]
            public string name;

            [DataMember]
            public int port;

            [DataMember]
            public StreamBroadcast[] outer_streams;

            [DataMember]
            public bool is_recording_active;

        }

        public virtual void Dispose()
        {
            executor.Dispose();
        }

        private void ProcessRefreshResponse(object response, Exception error)
        {
            if (error != null)
            {
                if (OnRefresh != null)
                {
                    OnRefresh(null);
                }
                refreshInProgress = false;
                return;
            }
            var streams = (StreamData[])response;
            var sources = new List<SourceEntry>();
            foreach (StreamData stream in streams)
            {
                var e = new SourceEntry
                {
                    name = stream.name,
                    uri = String.Format("http://{0}:{1}", host, stream.port),
                    port = stream.port,
                    isRecording = stream.is_recording_active
                };
                foreach (var bc in stream.outer_streams)
                {
                    SourceEntry.Broadcast.Status status;
                    if (bc.state >= (int)SourceEntry.Broadcast.Status.UNKNOWN)
                    {
                        status = SourceEntry.Broadcast.Status.UNKNOWN;
                    }
                    else
                    {
                        status = (SourceEntry.Broadcast.Status)bc.state;
                    }
                    e.broadcasts.Add(new SourceEntry.Broadcast
                    {
                        url = bc.url,
                        status = status,
                        statusMsg = bc.state_msg,
                        type = bc.type,
                        errorCode = bc.error_code
                    });
                }
                sources.Add(e);
            }
            if (OnRefresh != null)
            {
                OnRefresh(sources);
            }
            refreshInProgress = false;
        }

    }

}

