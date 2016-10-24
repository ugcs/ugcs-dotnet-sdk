using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Sdk.Tasks;
using Services.DTO;
using Services.Exceptions;
using Services.Interfaces;
using Services.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Services.SdkServices.ServerModels;

namespace Services.SdkServices
{
    public class Connect : IConnect
    {
        ILogger logger = new Logger(typeof(Connect));
        private MessageExecutor _executor;
        private AuthorizeHciRequest _request;
        private TcpClient _tcpClient;

        public MessageExecutor Executor
        {
            get
            {
                return _executor;
            }
        }
        public AuthorizeHciResponse AuthorizeHciResponse { get; set; }
        public LoginResponse LoginResponce { get; set; }

        public Connect(MessageExecutor executor,
            AuthorizeHciRequest request,
            TcpClient tcpClient)
        {
            _executor = executor;
            _request = request;
            _tcpClient = tcpClient;
        }

        /// <summary>
        /// Async authorization in UCS server
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <param name="onConnect">on connection callback without paramaters</param>
        /// <returns></returns>
        public async Task ConnectUgcs(String login, String password, System.Action onConnect)
        {
            var task = Task<TheadExceptionModel>.Factory.StartNew(() =>
            {
                LoginRequest loginRequest = new LoginRequest();
                loginRequest.UserLogin = login;
                loginRequest.UserPassword = password;

                _request.ClientId = -1;
                var future = _executor.Submit<AuthorizeHciResponse>(_request);
                if (future.Exception != null)
                {
                    return new TheadExceptionModel() { Message = future.Exception.Message, Status = 400 };
                }
                AuthorizeHciResponse = future.Value;

                loginRequest.ClientId = AuthorizeHciResponse.ClientId;
                LoginResponce = (LoginResponse)_executor.Submit<LoginResponse>(loginRequest).Value;
                if (LoginResponce == null || LoginResponce.User == null)
                {
                    return new TheadExceptionModel() { Message = "Invalid login or password", Status = 300 };
                }
                return new TheadExceptionModel() { Message = "OK", Status = 200 };
            });
            await task.ContinueWith((result) =>
            {
                if (result.Exception != null)
                {
                    result.Exception.Data.Clear();
                    logger.LogException(result.Exception);
                    throw new ConnectionException(result.Exception.Message);
                }
                if (result.Result.Status == 200)
                {
                    onConnect();
                }
                else if (result.Result.Status == 300)
                {
                    logger.LogError(result.Result.Message);
                    throw new ConnectionException(result.Result.Message);
                }
                else if (result.Result.Status == 400)
                {
                    logger.LogError("Exception occured: " + result.Result.Message);
                    throw new ConnectionException(result.Result.Message);
                }
            }).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
