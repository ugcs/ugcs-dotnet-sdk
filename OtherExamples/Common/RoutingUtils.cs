using System;
using System.Collections.Generic;
using System.Text;
using UGCS.Sdk.Protocol.Encoding;
using Action = UGCS.Sdk.Protocol.Encoding.Action;

namespace UgCS.SDK.Examples.Common
{
    public static class RoutingUtils
    {
        public static ProcessedRoute Process(this UcsFacade ucs, Route r)
        {
            ProcessRouteRequest req = new ProcessRouteRequest
            {
                ClientId = ucs.ClientId,
                Route = r
            };

            var res = ucs.Execute<ProcessRouteResponse>(req);
            return res.ProcessedRoute;
        }

        /// <summary>
        /// Enumerates all processed segments and collect waypoints into a list.
        /// </summary>
        /// <param name="route"></param>
        /// <returns>All waypoints of the route in the corresponding order.</returns>
        public static IList<Waypoint> GetWaypoints(this ProcessedRoute route)
        {
            var waypoints = new List<Waypoint>();

            foreach (ProcessedSegment segment in route.Segments)
            {
                if (segment == null || !segment.StatusSpecified ||
                    segment.Status == RouteProcessingStatus.RPS_INVALID)
                {
                    continue;
                }

                IEnumerable<Action> segmentActions = segment.OptimizedActions;
                if (segmentActions == null)
                    continue;

                foreach (Action action in segmentActions)
                {
                    if (action.Waypoint == null)
                        continue;

                    waypoints.Add(action.Waypoint);
                }
            }

            return waypoints;
        }

        public static bool HasErrors(this ProcessedRoute route)
        {
            foreach (var s in route.Segments)
            {
                if (s.StatusSpecified && s.Status == RouteProcessingStatus.RPS_INVALID)
                    return true;
            }
            return false;
        }
    }
}
