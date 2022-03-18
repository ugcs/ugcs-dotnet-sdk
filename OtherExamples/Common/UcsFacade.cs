using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        private NotificationListener _notificationListener;


        public event EventHandler Disconnected;


        public int ClientId { get; }


        private UcsFacade(TcpClient ucsConection, MessageExecutor executor, int clientId)
        {
            _ucsConnection = ucsConection;
            _executor = executor;
            ClientId = clientId;

            _notificationListener = new NotificationListener();
            executor.Receiver.AddListener(-1, _notificationListener);

            _ucsConnection.Session.Disconnected += ucs_Disconnected;
        }

        private void ucs_Disconnected(object sender, EventArgs e)
        {
            Disconnected?.Invoke(this, e);
        }

        public Task<TResponse> ExecuteAsync<TResponse>(IExtensible request)
            where TResponse : IExtensible
        {
            var tcs = new TaskCompletionSource<TResponse>();
            var future = _executor.Submit<TResponse>(request, r =>
            {
                if (r.Exception != null)
                    tcs.SetException(r.Exception);
                tcs.SetResult((TResponse)r.Value);
            });
            return tcs.Task;
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

        /// <param name="token">Use it in <see cref="Unsubscribe(SubscriptionToken)"/>.</param>
        public void Subscribe(EventSubscriptionWrapper subscription, NotificationHandler handler,
            out SubscriptionToken token)
        {
            var resp = Execute<SubscribeEventResponse>(
                new SubscribeEventRequest
                {
                    ClientId = this.ClientId,
                    Subscription = subscription
                });

            token = new SubscriptionToken(resp.SubscriptionId,
                handler,
                subscription);
            _notificationListener.AddSubscription(token);
        }

        public void Unsubscribe(SubscriptionToken token)
        {
            _notificationListener.RemoveSubscription(token, out bool removedLastForId);
        }

        public static UcsFacade connectToUcs(string host, int port = 3334, string login = "admin", string password = "admin")
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

        public static Task<UcsFacade> connectToUcsAsync(string host, int port, string login, string password)
        {
            return Task.Run(() => connectToUcs(host, port, login, password));
        }


        public void Dispose()
        {
            if (_isDisposed)
                return;

            _ucsConnection.Session.Disconnected -= ucs_Disconnected;

            _executor.Close();
            _ucsConnection.Close();
            _ucsConnection.Dispose();
            _notificationListener.Dispose();

            _isDisposed = true;
        }
    }
}
