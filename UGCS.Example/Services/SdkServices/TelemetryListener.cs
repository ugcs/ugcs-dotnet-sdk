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
    public class TelemetryListener
    {
        public delegate void TelemetrySubscriptionCallback(int vehicleId, List<Telemetry> telemetry);
        private EventSubscriptionWrapper _eventSubscriptionWrapper;
        private TelemetrySubscription _telemetrySubscription;
        private IConnect _connect;
        private Dictionary<int, ServiceActionTelemetry> _telemetryDTOList = new Dictionary<int, ServiceActionTelemetry>();
        private NotificationListener _notificationListener;

        /// <summary>
        /// Telemetry callback reciever
        /// </summary>
        /// <param name="vehicleId">vehcile id</param>
        /// <param name="telemetry">list of received telemetry</param>
        private void _telemetryReceived(int vehicleId, List<Telemetry> telemetry)
        {
            foreach (Telemetry t in telemetry)
            {
                switch (t.Type)
                {
                        /**
                         * 
                         * Use this to setup available telemetry field
                         * 
                         */

                    case TelemetryType.TT_STATE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.VehicleState = Convert.ToInt32(t.Value);
                        break;
                    case TelemetryType.TT_CONTROL_MODE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.ControlMode = Convert.ToInt32(t.Value);
                        break;
                    case TelemetryType.TT_AGL_ALTITUDE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.AltitudeAGL = t.Value;
                        break;
                    case TelemetryType.TT_LATITUDE_GPS:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.LatitudeGPS = t.Value;
                        break;
                    case TelemetryType.TT_LONGITUDE_GPS:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.LongitudeGPS = t.Value;
                        break;
                    case TelemetryType.TT_GPS_SATELLITES_VISIBLE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.SattelitesVisible = t.Value;
                        break;
                    case TelemetryType.TT_ALTITUDE_GPS:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.Altitude = t.Value;
                        break;
                    case TelemetryType.TT_PAYLOAD_1_YAW:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.Camera1Yaw = t.Value;
                        break;
                    case TelemetryType.TT_PAYLOAD_1_TILT:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.Camera1Tilt = t.Value;
                        break;
                    case TelemetryType.TT_ELEVATION:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.Elevation = t.Value;
                        break;
                    case TelemetryType.TT_BATTERY_VOLTAGE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.BatteryValue = t.Value;
                        break;
                    case TelemetryType.TT_TELEMETRY_DROP_RATE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.DownLink = t.Value;
                        break;
                    case TelemetryType.TT_DOWNLINK_CONNECTED:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.DownLinkConnected = t.Value;
                        break;
                    case TelemetryType.TT_GPS_FIX_TYPE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.GPSFix = t.Value;
                        break;
                    case TelemetryType.TT_PITCH:
                        break;
                    case TelemetryType.TT_ROLL:
                        break;
                    case TelemetryType.TT_HEADING:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.Camera1Yaw = t.Value;
                        break;
                    case TelemetryType.TT_GROUND_SPEED_Y:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_Y = t.Value;
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed = (float)Math.Sqrt(Math.Pow(_telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_X, 2)
                                                                        + Math.Pow(_telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_Y, 2));
                        break;
                    case TelemetryType.TT_GROUND_SPEED_X:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_X = t.Value;
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed = (float)Math.Sqrt(Math.Pow(_telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_X, 2)
                                                                        + Math.Pow(_telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_Y, 2));
                        break;
                    case TelemetryType.TT_GROUND_SPEED_Z:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.VerticalSpeed = t.Value;
                        break;
                    case TelemetryType.TT_AIR_SPEED:
                        break;
                    case TelemetryType.TT_LATITUDE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.Latitude = t.Value;
                        break;
                    case TelemetryType.TT_LONGITUDE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.Longitude = t.Value;
                        break;
                }
                if (_telemetryDTOList[vehicleId].Callback != null)
                {
                    _telemetryDTOList[vehicleId].Callback(_telemetryDTOList[vehicleId].ServiceTelemetryDTO, vehicleId);
                }
            }

        }

        /// <summary>
        /// Auto add vehicle to telemetry listener
        /// </summary>
        /// <param name="vehicleId"></param>
        private void _autoAdd(int vehicleId)
        {
            if (!_telemetryDTOList.ContainsKey(vehicleId))
            {
                _telemetryDTOList.Add(vehicleId, new ServiceActionTelemetry()
                {
                    ServiceTelemetryDTO = new ServiceTelemetryDTO()
                });
            }
        }

        /// <summary>
        /// Telemetry event handler
        /// </summary>
        /// <param name="callback">Subscription callback handler</param>
        /// <returns>Notification handler</returns>
        private NotificationHandler _getTelemetryNotificationHandler(TelemetrySubscriptionCallback callback)
        {
            return notification =>
            {
                TelemetryEvent telemetryEvent = notification.Event.TelemetryEvent;
                callback(telemetryEvent.Vehicle.Id, telemetryEvent.Telemetry);
            };
        }

        public TelemetryListener(EventSubscriptionWrapper espw, TelemetrySubscription ts, IConnect connect, NotificationListener notificationListener)
        {
            _connect = connect;
            _eventSubscriptionWrapper = espw;
            _telemetrySubscription = ts;
            _notificationListener = notificationListener;
        }

        /// <summary>
        /// Example how to activate subscription to all vehicles telemetry
        /// </summary>
        public void SubscribeTelemtry() 
        {
            _eventSubscriptionWrapper.TelemetrySubscription = _telemetrySubscription;

            SubscribeEventRequest requestEvent = new SubscribeEventRequest();
            requestEvent.ClientId = _connect.AuthorizeHciResponse.ClientId;

            requestEvent.Subscription = _eventSubscriptionWrapper;
            var responce = _connect.Executor.Submit<SubscribeEventResponse>(requestEvent);
            var subscribeEventResponse = responce.Value;

            SubscriptionToken st = new SubscriptionToken(subscribeEventResponse.SubscriptionId, _getTelemetryNotificationHandler(
                (vehicleId, telemetry) =>
                {
                    if (!_telemetryDTOList.ContainsKey(vehicleId))
                    {
                        _autoAdd(vehicleId);
                    }
                    _telemetryReceived(vehicleId, telemetry);
                }
                ), _eventSubscriptionWrapper);
            _notificationListener.AddSubscription(st);
           
        }

        /// <summary>
        /// Adds vehicle to telemetry listener
        /// </summary>
        /// <param name="vehicleId">vehcile id</param>
        /// <param name="callBack">callback for telemetry</param>
        public void AddVehicleIdTolistener(int vehicleId, System.Action<ServiceTelemetryDTO, int> callBack)
        {
            if (!_telemetryDTOList.ContainsKey(vehicleId))
            {
                _telemetryDTOList.Add(vehicleId, new ServiceActionTelemetry()
                {
                    ServiceTelemetryDTO = new ServiceTelemetryDTO(),
                    Callback = callBack
                });
            }
            else
            {
                _telemetryDTOList[vehicleId].Callback = callBack;
                callBack(_telemetryDTOList[vehicleId].ServiceTelemetryDTO, vehicleId);

            }
        }

        /// <summary>
        /// Removes 
        /// </summary>
        /// <param name="vehicleId"></param>
        public void RemoveVehicleIdTolistener(int vehicleId)
        {
            if (_telemetryDTOList.ContainsKey(vehicleId))
            {
                _telemetryDTOList[vehicleId].Callback = null;
            }
        }

        
    }
}
