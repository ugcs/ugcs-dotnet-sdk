using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Sdk.Tasks;

namespace UgCS.SDK.Examples.Common
{
    /// <summary>
    /// Simple facade to communicate with ucs.
    /// </summary>
    public sealed class UcsFacade : IDisposable
    {
        private readonly TcpClient _ucsConnection;
        private readonly MessageExecutor _executor;
        private bool _isDisposed = false;


        public int ClientId { get; }


        private UcsFacade(TcpClient ucsConection, MessageExecutor executor, int clientId)
        {
            _ucsConnection = ucsConection;
            _executor = executor;
            ClientId = clientId;
        }

        public TResponse Execute<TResponse>(IExtensible request)
            where TResponse : IExtensible
        {
            var future = _executor.Submit<TResponse>(request);
            future.Wait();
            if (future.Exception != null)
                throw new ApplicationException("Ucs request execution exception.", future.Exception);
            return future.Value;
        }

        public static UcsFacade connectToUcs(string host, int port, string login, string password)
        {
            var ucsConnection = new TcpClient();
            ucsConnection.Connect(host, port);

            var executor = new MessageExecutor(ucsConnection.Session, new InstantTaskScheduler());
            int clientId;

            try
            {
                //auth
                AuthorizeHciRequest request = new AuthorizeHciRequest();
                request.ClientId = -1;
                request.Locale = "en-US";
                var future = executor.Submit<AuthorizeHciResponse>(request);
                future.Wait();
                AuthorizeHciResponse AuthorizeHciResponse = future.Value;
                clientId = AuthorizeHciResponse.ClientId;

                //login
                LoginRequest loginRequest = new LoginRequest();
                loginRequest.UserLogin = login;
                loginRequest.UserPassword = password;
                loginRequest.ClientId = clientId;
                var loginResponcetask = executor.Submit<LoginResponse>(loginRequest);
                loginResponcetask.Wait();
            }
            catch
            {
                executor.Close();
                ucsConnection.Close();
                ucsConnection.Dispose();
                throw;
            }

            return new UcsFacade(ucsConnection, executor, clientId);
        }


        public void Dispose()
        {
            if (_isDisposed)
                return;

            _executor.Close();
            _ucsConnection.Close();
            _ucsConnection.Dispose();

            _isDisposed = true;
        }
    }
}
