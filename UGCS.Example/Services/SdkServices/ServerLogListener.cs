using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

namespace Services.SdkServices
{
    public class ServerLogListener : ServerObjectListener
    {
        private EventSubscriptionWrapper _eventSubscriptionWrapper;
        private ObjectModificationSubscription _objectNotificationSubscription;
        private IConnect _connect;
        private System.Action<VehicleLogEntry> _callback;

        public ServerLogListener(IConnect connect, NotificationListener notificationListener, EventSubscriptionWrapper eventSubscriptionWrapper)
        {
            _eventSubscriptionWrapper = eventSubscriptionWrapper;
            _notificationListener = notificationListener;
            _connect = connect;
        }
        
        /// <summary>
        /// Example how to activate subscription for server log messages
        /// </summary>
        /// <param name="es"></param>
        /// <param name="callback"></param>
        public void SubscribeLog(ObjectModificationSubscription es, System.Action<VehicleLogEntry> callback)
        {
            _callback = callback;
            _objectNotificationSubscription = es;
            _eventSubscriptionWrapper.ObjectModificationSubscription = _objectNotificationSubscription;

            SubscribeEventRequest requestEvent = new SubscribeEventRequest();
            requestEvent.ClientId = _connect.AuthorizeHciResponse.ClientId;

            requestEvent.Subscription = _eventSubscriptionWrapper;

            var responce = _connect.Executor.Submit<SubscribeEventResponse>(requestEvent);
            var subscribeEventResponse = responce.Value;

            SubscriptionToken st = new SubscriptionToken(subscribeEventResponse.SubscriptionId, _getObjectNotificationHandler<VehicleLogEntry>(
                (token, exception, log) =>
                {
                    if (token == ModificationType.MT_CREATE)
                    {
                        _oneMessageReceived(log);
                    }
                }
                ), _eventSubscriptionWrapper);
            _notificationListener.AddSubscription(st);
            _callback = callback;
           
        }

        public void _oneMessageReceived(VehicleLogEntry log)
        {
            if (_callback != null)
            {
                _callback(log);
            }
        }     

    }
}
