using ProtoBuf;
using Services.DTO;
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
    public class VehicleListener : ServerObjectListener
    {
        private EventSubscriptionWrapper _eventSubscriptionWrapper;
        private ObjectModificationSubscription _objectNotificationSubscription;
        private IConnect _connect;
        private Dictionary<int, System.Action<Vehicle>> _vehicleList = new Dictionary<int, System.Action<Vehicle>>();

        /// <summary>
        /// Add vehicle to listener
        /// </summary>
        /// <param name="vehicleId">vehicle id</param>
        /// <param name="callBack">callback for listener</param>
        private void AddVehicleIdTolistener(int vehicleId, System.Action<Vehicle> callBack)
        {
            if (!_vehicleList.ContainsKey(vehicleId))
            {
                _vehicleList.Add(vehicleId, callBack);
            }
        }

        public VehicleListener(EventSubscriptionWrapper espw, IConnect connect, NotificationListener notificationListener)
        {
            _connect = connect;
            _eventSubscriptionWrapper = espw;
            _notificationListener = notificationListener;
        }

        /// <summary>
        /// Example how to activate subscription o vehicle modifications
        /// </summary>
        /// <param name="es">ObjectModification with vehicle object id</param>
        /// <param name="callBack">callback with received event</param>
        public void SubscribeVehicle(ObjectModificationSubscription es, System.Action<Vehicle> callBack)
        {
            _objectNotificationSubscription = es;
            _eventSubscriptionWrapper.ObjectModificationSubscription = _objectNotificationSubscription;

            SubscribeEventRequest requestEvent = new SubscribeEventRequest();
            requestEvent.ClientId = _connect.AuthorizeHciResponse.ClientId;

            requestEvent.Subscription = _eventSubscriptionWrapper;

            var responce = _connect.Executor.Submit<SubscribeEventResponse>(requestEvent);
            var subscribeEventResponse = responce.Value;

            SubscriptionToken st = new SubscriptionToken(subscribeEventResponse.SubscriptionId, _getObjectNotificationHandler<Vehicle>(
                (token, exception, vehicle) =>
                {
                    if (token == ModificationType.MT_UPDATE && _vehicleList.ContainsKey(vehicle.Id))
                    {
                        _messageReceived(vehicle, _vehicleList[vehicle.Id]);
                    }
                }
                ), _eventSubscriptionWrapper);
            _notificationListener.AddSubscription(st);
            tokens.Add(st);
            AddVehicleIdTolistener(es.ObjectId, callBack);
           
        }

        /// <summary>
        /// Point where modificated vehicle object is received
        /// </summary>
        /// <param name="vehicle">modificated vehicle object</param>
        /// <param name="callback">callback for modificated vehcile object</param>
        public void _messageReceived(Vehicle vehicle, System.Action<Vehicle> callback)
        {
            callback(vehicle);
        }
    }
}
