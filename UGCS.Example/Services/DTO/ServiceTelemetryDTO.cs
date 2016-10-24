using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ServiceTelemetryDTO
    {
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public float? VerticalSpeed { get; set; }
        public float? GroundSpeed { get; set; }
        public float GroundSpeed_X { get; set; }
        public float GroundSpeed_Y { get; set; }
        public float? AltitudeAGL { get; set; }
        public float? LatitudeGPS { get; set; }
        public float? LongitudeGPS { get; set; }
        public float? SattelitesVisible { get; set; }
        public float Altitude { get; set; }
        public float? Camera1Yaw { get; set; }
        public float? Camera1Tilt { get; set; }
        public float Elevation { get; set; }
        public float? DownLink { get; set; }
        public float DownLinkConnected { get; set; }
        public float GPSFix { get; set; }
        public float WaypointNumber { get; set; } 
        public int VehicleState { get; set; }
        public int ControlMode { get; set; }
        public float? BatteryValue { get; set; }
        
    }
}
