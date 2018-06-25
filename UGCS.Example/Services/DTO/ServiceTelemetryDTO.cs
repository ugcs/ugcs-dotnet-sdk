using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ServiceTelemetryDTO
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? VerticalSpeed { get; set; }
        public double? GroundSpeed { get; set; }
        public double GroundSpeed_X { get; set; }
        public double GroundSpeed_Y { get; set; }
        public double? AltitudeAGL { get; set; }
        public float? LatitudeGPS { get; set; }
        public float? LongitudeGPS { get; set; }
        public float? SattelitesVisible { get; set; }
        public float Altitude { get; set; }
        public float? Camera1Yaw { get; set; }
        public float? Camera1Tilt { get; set; }
        public double Elevation { get; set; }
        public float? DownLink { get; set; }
        public bool DownLinkConnected { get; set; }
        public float GPSFix { get; set; }
        public long? WaypointNumber { get; set; } 
        public int VehicleState { get; set; }
        public long ControlMode { get; set; }
        public float? BatteryValue { get; set; }
        
    }
}
