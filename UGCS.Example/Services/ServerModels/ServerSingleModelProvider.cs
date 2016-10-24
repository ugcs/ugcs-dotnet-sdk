using ProtoBuf;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.SdkServices.ServerModels
{
    public abstract class ServerSingleModelProvider<T, TReq, TResp>
        where T : new()
        where TReq : IExtensible
        where TResp : IExtensible
    {
        private static object _syncRoot = new Object();
        private class ProviderCreator
        {
            private ProviderCreator() { }
            internal static readonly T instance = new T();
        }

        protected IConnect _connection;
        protected TResp _response;
        protected abstract TReq Request { get; }
        protected static T _instance;

        public ServerSingleModelProvider() { }
        public ServerSingleModelProvider(IConnect connection)
        {
            _connection = connection;
            Init();
        }

        protected void Init() 
        {
            var response = _connection.Executor.Submit<TResp>(Request);
            response.Wait();
            _response = response.Value;
        }

        public static T Instance
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = ProviderCreator.instance;
                    }
                }
                return _instance;
            }
        }
    }
}
