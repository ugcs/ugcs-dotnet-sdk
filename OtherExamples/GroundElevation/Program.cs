using System;
using UGCS.Sdk.Protocol.Encoding;
using System.Linq;
using System.Diagnostics;

namespace GroundElevation
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ucsHost = "localhost";
            const int ucsPort = 3334;

            Console.WriteLine($"Connecting to ucs at '{ucsHost}:{ucsPort}'...");
            using (UcsFacade ucs = UcsFacade.connectToUcs(ucsHost, ucsPort, "admin", "admin"))
            {
                Console.WriteLine("Connected to ucs.");
                string format = "{0:N7} {1:N7} {2:N2} m";

                double lat = 46.7616336;
                double lon = 8.3226754;
                double groundElevation = getGroundElevation(ucs, lat, lon);
                Console.WriteLine(format, lat, lon, groundElevation);

                lat = 46.7643946;
                lon = 8.3201203;
                groundElevation = getGroundElevation(ucs, lat, lon);
                Console.WriteLine(format, lat, lon, groundElevation);
            }
        }

        // lat, lon in degrees.
        private static double getGroundElevation(UcsFacade ucs, double lat, double lon)
        {
            var req = new GetElevationProfileRequest
            {
                ClientId = ucs.ClientId,
                // We don't want to get intermediate points
                SamplingStep = double.MaxValue
            };
            req.Locations.Add(
                new Wgs84LocationDto
                {
                    Latitude = toRadians(lat),
                    Longitude = toRadians(lon)
                });

            var res = ucs.Execute<GetElevationProfileResponse>(req);
            Debug.Assert(res.Elevations.Count == 1);
            return res.Elevations.Single();
        }

        private static double toRadians(double degrees)
        {
            return degrees / 180.0 * Math.PI;
        }
    }
}
