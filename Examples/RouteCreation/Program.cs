using System;
using UGCS.Sdk.Protocol.Encoding;
using System.Linq;
using System.Diagnostics;
using UgCS.SDK.Examples.Common;

namespace UgCS.SDK.Examples.GroundElevation
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ucsHost = "localhost";
            const int ucsPort = 3334;

            Console.WriteLine("Creating a route...");
            Route route = new Route();
            // Set mandatory route parameters, e.g., NFZ use
            // In case of no license purchased, airport NFZ must be on, and custom NFZ must be off
            route.CheckCustomNfz = false;
            route.CheckAerodromeNfz = true;

            SegmentDefinition newWaypointSegment = CreateWaypoint();
            SegmentDefinition newAreaScanSegment = CreateAreaScan();

            // Now add segments
            route.Segments.Add(newWaypointSegment);
            route.Segments.Add(newAreaScanSegment);

            // And set their order
            for (int i = 0; i < route.Segments.Count; i++)
                route.Segments[i].Order = i;

            Console.WriteLine($"Connecting to ucs at '{ucsHost}:{ucsPort}'...");
            using (UcsFacade ucs = UcsFacade.connectToUcs(ucsHost, ucsPort, "admin", "admin"))
            {
                Console.WriteLine("Connected to ucs.");

                // We need to set a vehicle profile for the route
                Console.WriteLine("Retrieving vehicle list...");
                Vehicle vehicle = ucs.List<Vehicle>().FirstOrDefault(v => v.Name == "EMU-101");
                if (vehicle == null)
                {
                    Console.WriteLine("Could not find emucopter. Exiting.");
                    return;
                }

                route.VehicleProfile = vehicle.Profile;

                // Now it is time to process the route before we can upload it anywhere
                ProcessedRoute processedRoute = ucs.Process(route);

                if(processedRoute.Segments.Any(s => s.Status != RouteProcessingStatus.RPS_PROCESSED))
                {
                    Console.WriteLine("Errors processing route. Exiting.");
                    return;
                }

                Console.WriteLine("Route successfully processed.");
                // Now this route can be uploaded to a craft
            }
        }

        private static SegmentDefinition CreateWaypoint()
        {
            // Create a segment of type Waypoint
            SegmentDefinition newWaypointSegment = new SegmentDefinition
            {
                Uuid = Guid.NewGuid().ToString(), // This is IMPORTANT!
                AlgorithmClassName = "com.ugcs.ucs.service.routing.impl.WaypointAlgorithm",
            };

            // Set the figure defining segment geometry, in case of waypoint it is FT_POINT
            newWaypointSegment.Figure = new Figure { Type = FigureType.FT_POINT };
            // Add a point to figure, in this case, in AGL mode
            // Latitude and Longitude are in Radians
            newWaypointSegment.Figure.Points.Add(CreateFigurePoint(0.816397042263197, 0.145668814620505, 50, AltitudeType.AT_AGL));

            // Fill out parameters for the segment
            newWaypointSegment.ParameterValues.Add(CreateParameterValue("speed", "5"));
            newWaypointSegment.ParameterValues.Add(CreateParameterValue("wpTurnType", "SPLINE"));
            newWaypointSegment.ParameterValues.Add(CreateParameterValue("avoidObstacles", "True"));
            newWaypointSegment.ParameterValues.Add(CreateParameterValue("avoidTerrain", "True"));
            newWaypointSegment.ParameterValues.Add(CreateParameterValue("altitudeType", "AGL"));
            return newWaypointSegment;
        }

        private static SegmentDefinition CreateAreaScan()
        {
            // Create a segment of type AreaScan
            SegmentDefinition newAreaScanSegment = new SegmentDefinition
            {
                Uuid = Guid.NewGuid().ToString(), // This is IMPORTANT!
                AlgorithmClassName = "com.ugcs.ucs.service.routing.impl.AreaScanAlgorithm",
            };

            // Set the figure defining segment geometry, in case of area scan it is FT_POLYGON
            newAreaScanSegment.Figure = new Figure { Type = FigureType.FT_POLYGON };
            // Now we need to add at least three points, let's try AMSL now
            // Latitude and Longitude are in Radians
            newAreaScanSegment.Figure.Points.Add(CreateFigurePoint(0.816391932179384, 0.14568959218058, 1880, AltitudeType.AT_WGS84));
            newAreaScanSegment.Figure.Points.Add(CreateFigurePoint(0.816378886900021, 0.145691705177651, 1880, AltitudeType.AT_WGS84));
            newAreaScanSegment.Figure.Points.Add(CreateFigurePoint(0.816382940878921, 0.145680936195178, 1880, AltitudeType.AT_WGS84));
            // Since this is a polygon, it needs to be closed, therefore we need to add the last point which equals the first one
            newAreaScanSegment.Figure.Points.Add(CreateFigurePoint(0.816391932179384, 0.14568959218058, 1880, AltitudeType.AT_WGS84));

            // Fill out parameters for the segment
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("speed", "5"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("wpTurnType", "STOP_AND_TURN"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("height", "1880"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("altitudeType", "WGS84"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("sideDistance", "5"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("directionAngle", "0"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("avoidObstacles", "True"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("actionExecution", "ACTIONS_EVERY_POINT"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("overshoot", ""));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("overshootSpeed", ""));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("areaScanAllowPartialCalculation", "False"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("tolerance", "3"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("noActionsAtLastPoint", "True"));
            newAreaScanSegment.ParameterValues.Add(CreateParameterValue("doubleGrid", "False"));
            return newAreaScanSegment;
        }

        private static ParameterValue CreateParameterValue(string name, string value)
        {
            return new ParameterValue()
            {
                Value = value,
                Name = name
            };
        }

        private static FigurePoint CreateFigurePoint(double latitude, double longitude, double altitude, AltitudeType altitudeOrigin)
        {
            FigurePoint result = new FigurePoint
            {
                Latitude = latitude,
                Longitude = longitude,
            };

            if (altitudeOrigin == AltitudeType.AT_WGS84)
                result.Wgs84Altitude = altitude;
            else
                result.AglAltitude = altitude;
            result.AltitudeType = altitudeOrigin;
            return result;
        }
    }
}
