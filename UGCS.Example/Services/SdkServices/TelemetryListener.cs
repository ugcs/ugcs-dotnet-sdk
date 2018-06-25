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
        /// Receive telemetry value or null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="telemetryValue"></param>
        /// <returns></returns>
        private Nullable<T> GetValueOrNull<T>(Value telemetryValue) where T : struct
        {
            Nullable<T> returnValue = null;
            if (telemetryValue != null)
            {
                if (typeof(T) == typeof(float))
                {
                    returnValue = (T)Convert.ChangeType(telemetryValue.FloatValue, typeof(T));
                }
                if (typeof(T) == typeof(long))
                {
                    returnValue = (T)Convert.ChangeType(telemetryValue.LongValue, typeof(T));
                }
                if (typeof(T) == typeof(int))
                {
                    returnValue = (T)Convert.ChangeType(telemetryValue.IntValue, typeof(T));
                }
                if (typeof(T) == typeof(bool))
                {
                    returnValue = (T)Convert.ChangeType(telemetryValue.BoolValue, typeof(T));
                }
                if (typeof(T) == typeof(double))
                {
                    returnValue = (T)Convert.ChangeType(telemetryValue.DoubleValue, typeof(T));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Receive telemetry value or default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="telemetryValue"></param>
        /// <returns></returns>
        private T GetValueOrDefault<T>(Value telemetryValue) where T : struct
        {
            return GetValueOrNull<T>(telemetryValue).GetValueOrDefault();
        }

        /// <summary>
        /// Telemetry callback reciever
        /// </summary>
        /// <param name="vehicleId">vehcile id</param>
        /// <param name="telemetry">list of received telemetry</param>
        private void _telemetryReceived(int vehicleId, List<Telemetry> telemetry)
        {
            try
            {
                foreach (Telemetry t in telemetry)
                {
                    if (t.TelemetryField.Code == "is_armed" && t.TelemetryField.Semantic == Semantic.S_BOOL && t.TelemetryField.Subsystem == Subsystem.S_FLIGHT_CONTROLLER)
                    {
                        //case TelemetryType.TT_STATE:
                        if (t.Value == null)
                        {
                            _telemetryDTOList[vehicleId].ServiceTelemetryDTO.VehicleState = 0;
                        }
                        else
                        {
                            _telemetryDTOList[vehicleId].ServiceTelemetryDTO.VehicleState = GetValueOrDefault<bool>(t.Value) ? 2 : 1;
                        }
                    }
                    if (t.TelemetryField.Code == "control_mode" && t.TelemetryField.Semantic == Semantic.S_CONTROL_MODE && t.TelemetryField.Subsystem == Subsystem.S_FLIGHT_CONTROLLER)
                    {
                        //case TelemetryType.TT_CONTROL_MODE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.ControlMode = GetValueOrDefault<long>(t.Value);
                    }
                    if (t.TelemetryField.Code == "altitude_agl" && t.TelemetryField.Semantic == Semantic.S_ALTITUDE_AGL && t.TelemetryField.Subsystem == Subsystem.S_CONTROL_SERVER)
                    {
                        //case TelemetryType.TT_AGL_ALTITUDE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.AltitudeAGL = GetValueOrNull<double>(t.Value);
                    }
                    if (t.TelemetryField.Code == "ground_elevation" && t.TelemetryField.Semantic == Semantic.S_GROUND_ELEVATION && t.TelemetryField.Subsystem == Subsystem.S_CONTROL_SERVER)
                    {
                        //case TelemetryType.TT_ELEVATION:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.Elevation = GetValueOrDefault<double>(t.Value);
                    }
                    if (t.TelemetryField.Code == "gcs_link_quality" && t.TelemetryField.Semantic == Semantic.S_GCS_LINK_QUALITY && t.TelemetryField.Subsystem == Subsystem.S_FLIGHT_CONTROLLER)
                    {
                        //case TelemetryType.TT_TELEMETRY_DROP_RATE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.DownLink = GetValueOrNull<float>(t.Value);
                    }
                    if (t.TelemetryField.Code == "downlink_present" && t.TelemetryField.Semantic == Semantic.S_BOOL && t.TelemetryField.Subsystem == Subsystem.S_FLIGHT_CONTROLLER)
                    {
                        //case TelemetryType.TT_DOWNLINK_CONNECTED:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.DownLinkConnected = GetValueOrDefault<bool>(t.Value);
                    }
                    if (t.TelemetryField.Code == "gps_fix" && t.TelemetryField.Semantic == Semantic.S_GPS_FIX_TYPE && t.TelemetryField.Subsystem == Subsystem.S_FLIGHT_CONTROLLER)
                    {
                        //case TelemetryType.TT_GPS_FIX_TYPE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.GPSFix = GetValueOrDefault<long>(t.Value);
                    }
                    if (t.TelemetryField.Code == "current_waypoint_index" && t.TelemetryField.Semantic == Semantic.S_NUMERIC && t.TelemetryField.Subsystem == Subsystem.S_CONTROL_SERVER)
                    {
                        //case TelemetryType.TT_WAYPOINT_NUMBER:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.WaypointNumber = GetValueOrNull<long>(t.Value);
                    }
                    if (t.TelemetryField.Code == "ground_speed_y" && t.TelemetryField.Semantic == Semantic.S_NUMERIC && t.TelemetryField.Subsystem == Subsystem.S_CONTROL_SERVER)
                    {
                        //case TelemetryType.TT_GROUND_SPEED_Y:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_Y = GetValueOrDefault<double>(t.Value);
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed = Math.Sqrt(Math.Pow(_telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_X, 2)
                                                                        + Math.Pow(_telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_Y, 2));
                    }
                    if (t.TelemetryField.Code == "ground_speed_x" && t.TelemetryField.Semantic == Semantic.S_NUMERIC && t.TelemetryField.Subsystem == Subsystem.S_CONTROL_SERVER)
                    {
                        //case TelemetryType.TT_GROUND_SPEED_X:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_X = GetValueOrDefault<double>(t.Value);
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed = Math.Sqrt(Math.Pow(_telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_X, 2)
                                                                        + Math.Pow(_telemetryDTOList[vehicleId].ServiceTelemetryDTO.GroundSpeed_Y, 2));
                    }
                    if (t.TelemetryField.Code == "ground_speed_z" && t.TelemetryField.Semantic == Semantic.S_NUMERIC && t.TelemetryField.Subsystem == Subsystem.S_CONTROL_SERVER)
                    {
                        //case TelemetryType.TT_GROUND_SPEED_Z:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.VerticalSpeed = GetValueOrNull<double>(t.Value);
                    }

                    if (t.TelemetryField.Code == "main_voltage" && t.TelemetryField.Semantic == Semantic.S_VOLTAGE && t.TelemetryField.Subsystem == Subsystem.S_FLIGHT_CONTROLLER)
                    {
                        //case TelemetryType.TT_BATTERY_VOLTAGE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.BatteryValue = GetValueOrNull<float>(t.Value);
                    }
                    if (t.TelemetryField.Code == "latitude" && t.TelemetryField.Semantic == Semantic.S_LATITUDE && t.TelemetryField.Subsystem == Subsystem.S_FLIGHT_CONTROLLER)
                    {
                        //case TelemetryType.TT_LATITUDE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.Latitude = GetValueOrNull<double>(t.Value);
                    }
                    if (t.TelemetryField.Code == "longitude" && t.TelemetryField.Semantic == Semantic.S_LONGITUDE && t.TelemetryField.Subsystem == Subsystem.S_FLIGHT_CONTROLLER)
                    {
                        //case TelemetryType.TT_LONGITUDE:
                        _telemetryDTOList[vehicleId].ServiceTelemetryDTO.Longitude = GetValueOrNull<double>(t.Value);
                    }
                }
                if (_telemetryDTOList[vehicleId].Callback != null)
                {
                    _telemetryDTOList[vehicleId].Callback(_telemetryDTOList[vehicleId].ServiceTelemetryDTO, vehicleId);
                }
            }
            catch
            {
                //Silent
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
                        _telemetryDTOList.Add(vehicleId, new ServiceActionTelemetry()
                        {
                            ServiceTelemetryDTO = new ServiceTelemetryDTO()
                        });
                    }
                    _telemetryReceived(vehicleId, telemetry);
                }), _eventSubscriptionWrapper);
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
