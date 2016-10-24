using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Example.Enums;

namespace UGCS.Example.Models
{
    public class ClientTelemetry : Caliburn.Micro.PropertyChangedBase
    {
        public Vehicle Vehicle { get; set; }
        private float? _latitude;
        public float? Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                _latitude = value;
                NotifyOfPropertyChange(() => Latitude);
            }
        }
        private float? _longitude;
        public float? Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                _longitude = value;
                NotifyOfPropertyChange(() => Longitude);
            }
        }

        private float? _verticalSpeed;
        public float? VerticalSpeed
        {
            get
            {
                return _verticalSpeed;
            }
            set
            {
                _verticalSpeed = value;
                NotifyOfPropertyChange(() => VerticalSpeed);
            }
        }
        private float? _sattelitesVisible;
        public float? SattelitesVisible
        {
            get
            {
                return _sattelitesVisible;
            }
            set
            {
                _sattelitesVisible = value;
                NotifyOfPropertyChange(() => SattelitesVisible);
                NotifyOfPropertyChange(() => VehicleSattelitesIndicator);
            }
        }
        private float? _groundSpeed;
        public float? GroundSpeed
        {
            get
            {
                return _groundSpeed;
            }
            set
            {
                _groundSpeed = value;
                NotifyOfPropertyChange(() => GroundSpeed);
            }
        }
        private float _altitudeSafe;
        public float AltitudeSafe
        {
            get
            {
                return (float)Math.Round(_altitudeSafe, 1);
            }
            set
            {
                _altitudeSafe = value;
            }
        }
        public bool IsOverSafe
        {
            get
            {
                return (AltitudeSafe > AltitudeAGL);
            }
        }

        private float? _altitudeAGL;
        public float? AltitudeAGL
        {
            get
            {
                return _altitudeAGL;
            }
            set
            {
                _altitudeAGL = value;
                NotifyOfPropertyChange(() => AltitudeAGL);
            }
        }

        public float? LatitudeGPS
        {
            get
            {
                return Latitude;
            }
        }
        public float? LongitudeGPS
        {
            get
            {
                return Longitude;
            }
        }

        private float? _downLink;
        public float? DownLink
        {
            get
            {
                if (_downLink == null)
                {
                    return _downLink;
                }
                return (float)Math.Round((100.0f - _downLink ?? 0), 0);
            }
            set
            {
                _downLink = value;
                NotifyOfPropertyChange(() => DownLink);
            }
        }

        private float _waypointNumber;
        public float WaypointNumber
        {
            get
            {
                return _waypointNumber;
            }
            set
            {

                _waypointNumber = value;
                NotifyOfPropertyChange(() => WaypointNumber);
            }
        }
        private float _downLinkConnected;
        public float DownLinkConnected
        {
            get
            {
                return _downLinkConnected;
            }
            set
            {

                _downLinkConnected = value;
                NotifyOfPropertyChange(() => DownLinkConnected);
            }
        }
        private float _GPSFix;
        public float GPSFix
        {
            get
            {
                return _GPSFix;
            }
            set
            {
                _GPSFix = value;
                NotifyOfPropertyChange(() => GPSFix);
            }
        }

        public bool Connected
        {
            get
            {
                return (_downLinkConnected > 0) ? true : false;
            }
        }


        private int _vehicleState;
        public int VehicleState
        {
            get
            {
                if (!Connected)
                {
                    return 0;
                }
                return _vehicleState;
            }
            set
            {
                _vehicleState = value;
                NotifyOfPropertyChange(() => VehicleState);
                NotifyOfPropertyChange(() => VehicleModeStatus);
                NotifyOfPropertyChange(() => VehicleMode);
            }
        }


        private int _controlMode;
        public int ControlMode
        {
            get
            {
                return _controlMode;
            }
            set
            {
                _controlMode = value;
                NotifyOfPropertyChange(() => ControlMode);
                NotifyOfPropertyChange(() => VehicleMode);
            }
        }

        private float? _batteryValue;
        public float? BatteryValue
        {
            get
            {
                return _batteryValue;
            }
            set
            {
                _batteryValue = value;
                NotifyOfPropertyChange(() => BatteryValue);
                NotifyOfPropertyChange(() => BatteryValueIndicator);

            }
        }
        

        public float Altitude { get; set; }
        public float Elevation { get; set; }

        private float? _camera1Yaw;
        public float? Camera1Yaw
        {
            get
            {
                return _camera1Yaw;
            }
            set
            {
                _camera1Yaw = value;
                NotifyOfPropertyChange(() => Camera1Yaw);

            }
        }

        private float? _camera1Tilt;
        public float? Camera1Tilt
        {
            get
            {
                return _camera1Tilt;

            }
            set
            {
                _camera1Tilt = value;
                NotifyOfPropertyChange(() => Camera1Tilt);

            }
        }

        public bool VehicleModeStatus
        {
            get
            {
                switch (_vehicleState)
                {
                    case 2:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public VehicleControlMode VehicleControlMode
        {
            get
            {
                switch (ControlMode)
                {
                    case 0:
                        return VehicleControlMode.MODE_UNKOWN;
                    case 1:
                        return VehicleControlMode.MODE_AUTO;
                    case 2:
                        return VehicleControlMode.MODE_MANUAL;
                    case 3:
                        return VehicleControlMode.MODE_GUIDED;
                    case 4:
                        return VehicleControlMode.MODE_JOYSTICK;
                    default:
                        return VehicleControlMode.MODE_UNKOWN;
                }
            }
        }

        public int? VehicleSattelitesIndicator
        {
            get
            {
                if (VehicleModeStatus == false)
                {
                    return null;
                }
                var lowGPS = Vehicle.Profile.Parameters.FirstOrDefault(x => x.Type == VehicleParameterType.VPT_LOW_GPS_SATELLITES).Value;
                var normalGPS = Vehicle.Profile.Parameters.FirstOrDefault(x => x.Type == VehicleParameterType.VPT_NORMAL_GPS_SATELLITES).Value;
                if (SattelitesVisible == null || SattelitesVisible.Value < lowGPS)
                {
                    return 0;
                }
                if (SattelitesVisible.Value >= normalGPS)
                {
                    return 2;
                }
                return 1;
            }
        }

        public int? BatteryValueIndicator
        {
            get
            {
                if (VehicleModeStatus == false)
                {
                    return null;
                }
                var lowBattery = Vehicle.Profile.Parameters.FirstOrDefault(x => x.Type == VehicleParameterType.VPT_NORMAL_BATTERY_VOLTAGE).Value;
                var normalBattery = Vehicle.Profile.Parameters.FirstOrDefault(x => x.Type == VehicleParameterType.VPT_LOW_BATTERY_VOLTAGE).Value;
                if (BatteryValue == null || BatteryValue.Value < lowBattery)
                {
                    return 0;
                }
                if (BatteryValue.Value >= normalBattery)
                {
                    return 2;
                }
                return 1;
            }
        }

        public String VehicleMode
        {
            get
            {

                if (!Connected)
                {
                    return "Unknown";
                }
                List<String> mode = new List<String>();
                switch (_vehicleState)
                {
                    case 2:
                        mode.Add("Armed");
                        break;
                    case 1:
                        mode.Add("Disarmed");
                        break;
                    default:
                        mode.Add("Unknown");
                        break;
                }
                switch (_controlMode)
                {
                    case 1:
                        mode.Add("Auto");
                        if (_waypointNumber > 0)
                        {
                            mode.Add("Flying to #" + Math.Round(_waypointNumber));
                        }
                        break;
                    case 2:
                        mode.Add("Manual");
                        break;
                    case 3:
                        mode.Add("Hold");
                        break;
                    case 0:
                        break;
                    case 5:
                        mode.Add("Auto");
                        if (_waypointNumber > 0)
                        {
                            mode.Add("Waiting at #" + Math.Round(_waypointNumber));
                        }
                        break;
                    case 6:
                        mode.Add("RTL");
                        break;
                    case 7:
                        mode.Add("Landing");
                        break;
                    default:
                        mode.Add("MODE:" + _controlMode);
                        break;
                }
                return String.Join(", ", mode);
            }
        }


        
    }
}
