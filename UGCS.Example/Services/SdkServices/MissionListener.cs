using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using ProtoBuf;
using Services.DTO;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SdkServices
{
    public class MissionListener : ServerObjectListener
    {
        private EventSubscriptionWrapper _eventSubscriptionWrapper;
        private ObjectModificationSubscription _objectNotificationSubscription;
        private IConnect _connect;
        private System.Action<Mission> _callback;

        public MissionListener(EventSubscriptionWrapper espw, IConnect connect, NotificationListener notificationListener)
        {
            _connect = connect;
            _eventSubscriptionWrapper = espw;
            _notificationListener = notificationListener;
        }

        /// <summary>
        /// Subscription for mission modification
        /// </summary>
        /// <param name="es">Subscriptiob object</param>
        /// <param name="callback">callback for telemetry subscription</param>
        public void SubscribeMission(ObjectModificationSubscription es, System.Action<Mission> callback)
        {
            UnsubscribeAll();
            _objectNotificationSubscription = es;
            _eventSubscriptionWrapper.ObjectModificationSubscription = _objectNotificationSubscription;

            SubscribeEventRequest requestEvent = new SubscribeEventRequest();
            requestEvent.ClientId = _connect.AuthorizeHciResponse.ClientId;

            requestEvent.Subscription = _eventSubscriptionWrapper;

            var responce = _connect.Executor.Submit<SubscribeEventResponse>(requestEvent);
            var subscribeEventResponse = responce.Value;

            SubscriptionToken st = new SubscriptionToken(subscribeEventResponse.SubscriptionId, _getObjectNotificationHandler<Mission>(
                (token, exception, mission) =>
                {
                    if (token == ModificationType.MT_UPDATE)
                    {
                        _messageReceived(mission);
                    }
                }
                ), _eventSubscriptionWrapper);
            _notificationListener.AddSubscription(st);
            tokens.Add(st);
            _callback = callback;

        }

        /// <summary>
        /// callback for mission modificatiob
        /// </summary>
        /// <param name="mission">modified mission object</param>
        public void _messageReceived(Mission mission)
        {
            if (_callback != null)
            {
                _callback(mission);
            }
        }
    }
}
