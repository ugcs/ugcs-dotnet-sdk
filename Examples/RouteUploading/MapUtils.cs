using MapControl;
using System.Collections.Generic;
using System.Linq;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint
{
    internal static class MapUtils
    {
        public static BoundingBox GetBoundingBox(this IEnumerable<Location> points)
        {
            double maxLat = points.Max(x => x.Latitude);
            double minLat = points.Min(x => x.Latitude);
            double maxLon = points.Max(x => x.Longitude);
            double minLon = points.Min(x => x.Longitude);

            return new BoundingBox(minLat, minLon, maxLat, maxLon);
        }

        public static BoundingBox Extend(this BoundingBox boundingBox, int bufferInMeters)
        {
            return new BoundingBox(
                boundingBox.South - bufferInMeters / MapProjection.Wgs84MetersPerDegree,
                boundingBox.West - bufferInMeters / MapProjection.Wgs84MetersPerDegree,
                boundingBox.North + bufferInMeters / MapProjection.Wgs84MetersPerDegree,
                boundingBox.East + bufferInMeters / MapProjection.Wgs84MetersPerDegree);
        }
    }
}
