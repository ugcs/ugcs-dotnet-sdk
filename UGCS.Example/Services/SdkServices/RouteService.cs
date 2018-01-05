using Services.DTO;
using Services.Interfaces;
using Services.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Sdk.Protocol;
using Services.Helpers;

namespace Services.SdkServices
{
    public class RouteService
    {
        private ILogger logger = new Logger(typeof(RouteService));
        private IConnect _connect;
        private MissionService _missionService;
        private MappingRequestService _mappingRequestService;

        public RouteService(IConnect connect, MissionService missionService, MappingRequestService mrs)
        {
            _connect = connect;
            _missionService = missionService;
            _mappingRequestService = mrs;
        }

        /// <summary>
        /// Minimu requirments to create waypoin
        /// </summary>
        /// <param name="route">Route where create waypoint</param>
        /// <param name="index">waypoint index</param>
        /// <param name="lat">Latitude in radians</param>
        /// <param name="lng">Longitude in radians</param>
        /// <returns></returns>
        public Route AddWaypoint(Route route, int index, double lat, double lng)
        {
            TraverseAlgorithm wpAlgorithm = _mappingRequestService.GetAlgoritmByClassName("com.ugcs.ucs.service.routing.impl.WaypointAlgorithm");
            SegmentDefinition newSegment = new SegmentDefinition
            {
                Uuid = Guid.NewGuid().ToString(),
                AlgorithmClassName = wpAlgorithm.ImplementationClass
            };
            newSegment.Figure = new Figure { Type = wpAlgorithm.FigureType };

            newSegment.ParameterValues.Add(new ParameterValue()
            {
                Name = "speed",
                Value = "5.0",
                ValueSpecified = true
            });
            newSegment.ParameterValues.Add(new ParameterValue()
            {
                Name = "wpTurnType",
                Value = "SPLINE",
                ValueSpecified = true
            });
            newSegment.ParameterValues.Add(new ParameterValue()
            {
                Name = "avoidObstacles",
                Value = "True",
                ValueSpecified = true
            });
            newSegment.ParameterValues.Add(new ParameterValue()
            {
                Name = "avoidTerrain",
                Value = "True",
                ValueSpecified = true
            });


            newSegment.Figure.Points.Add(new FigurePoint()
            {
                AglAltitude = 20,
                AglAltitudeSpecified = true,
                AltitudeType = AltitudeType.AT_AGL,
                AltitudeTypeSpecified = true,
                Latitude = lat, //0.99443566874164979,
                LatitudeSpecified = true,
                Longitude = lng, //0.42015588448045021,
                LongitudeSpecified = true,
                Order = 0,
                Wgs84Altitude = 0.0,
                Wgs84AltitudeSpecified = true
            });

            route.Segments.Insert(index, newSegment);
            return SaveUpdatedRoute(route);
        }

        /// <summary>
        /// Example how to create route on server with route parameters
        /// Minimum parameters is specified
        /// </summary>
        /// <param name="mission">Mission where create route</param>
        /// <param name="vehicleProfile">Profile for route</param>
        /// <param name="name">Route name</param>
        /// <returns></returns>
        public Route CreateNewRoute(Mission mission, VehicleProfile vehicleProfile, String name = "TestRoute")
        {

            var routeName = string.Format("{0} {1}", name, DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());

            Route route = new Route
            {
                CreationTime = ServiceHelpers.CreationTime(),
                Name = routeName,
                Mission = mission
            };

            ChangeRouteVehicleProfileRequest request = new ChangeRouteVehicleProfileRequest
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                Route = route,
                NewProfile = new VehicleProfile { Id = vehicleProfile.Id }
            };
            MessageFuture<ChangeRouteVehicleProfileResponse> future =
                _connect.Executor.Submit<ChangeRouteVehicleProfileResponse>(request);
            future.Wait();
            route = future.Value.Route;
            route.Mission = mission;
            route.HomeLocationSource = HomeLocationSource.HLS_FIRST_WAYPOINT;
            route.TrajectoryType = TrajectoryType.TT_STAIR;
            route.AltitudeType = AltitudeType.AT_AGL;
            route.MaxAltitude = 50.0;
            route.SafeAltitude = 3.0;
            route.CheckAerodromeNfz = false;
            route.CheckAerodromeNfzSpecified = true;
            route.InitialSpeed = 0.0;
            route.MaxSpeed = 0.0;
            route.CheckCustomNfz = false;
            route.CheckCustomNfzSpecified = true;
            route.Failsafes.Add(new Failsafe()
            {
                Action = FailsafeAction.FA_GO_HOME,
                ActionSpecified = true,
                Reason = FailsafeReason.FR_RC_LOST,
                ReasonSpecified = true
            });
            route.Failsafes.Add(new Failsafe()
            {
                Action = FailsafeAction.FA_LAND,
                ActionSpecified = true,
                Reason = FailsafeReason.FR_LOW_BATTERY,
                ReasonSpecified = true
            });
            route.Failsafes.Add(new Failsafe()
            {
                Action = FailsafeAction.FA_WAIT,
                ActionSpecified = true,
                Reason = FailsafeReason.FR_GPS_LOST,
                ReasonSpecified = true
            });
            route.Failsafes.Add(new Failsafe()
            {
                Action = FailsafeAction.FA_GO_HOME,
                ActionSpecified = true,
                Reason = FailsafeReason.FR_DATALINK_LOST,
                ReasonSpecified = true
            });


            return SaveUpdatedRoute(route);
        }

        /// <summary>
        /// Examp,e how create or update route on server
        /// </summary>
        /// <param name="route">modified route</param>
        /// <returns>Updated route from server</returns>
        public Route SaveUpdatedRoute(Route route)
        {
            CreateOrUpdateObjectRequest request = new CreateOrUpdateObjectRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                Object = new DomainObjectWrapper().Put(route, "Route"),
                WithComposites = true,
                ObjectType = "Route",
                AcquireLock = false
            };
            var task = _connect.Executor.Submit<CreateOrUpdateObjectResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
                throw new Exception("Save error: " + task.Exception.Message);
            }

            if (task.Value == null)
            {
                logger.LogWarningMessage("Could not save route info: " + route.Name);
                throw new Exception("Could not save route info: " + route.Name);
            }

            return task.Value.Object.Route;
        }

        /// <summary>
        /// Example how update route by id
        /// </summary>
        /// <param name="routeId">possible route id</param>
        /// <returns>Updated route by id</returns>
        public Route GetUpdatedRouteById(int routeId)
        {
            GetObjectRequest request = new GetObjectRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ObjectId = routeId,
                ObjectType = "Route",
                RefreshDependencies = true
            };

            request.RefreshExcludes.Add("Vehicle");
            request.RefreshExcludes.Add("PayloadProfile");
            request.RefreshExcludes.Add("Mission");

            var task = _connect.Executor.Submit<GetObjectResponse>(request);
            task.Wait();


            if (task.Value == null)
            {
                throw new Exception("Could not retrieve route info: " + routeId);
            }

            return task.Value.Object.Route;
        }

        /// <summary>
        /// Example how to calculate route by id
        /// </summary>
        /// <param name="routeId">route ID</param>
        /// <returns>Processed route</returns>
        public ProcessedRoute CalculateRouteById(int routeId)
        {
            var updatedRoute = GetUpdatedRouteById(routeId);
            ProcessRouteRequest request = new ProcessRouteRequest
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                Route = updatedRoute,
            };

            MessageFuture<ProcessRouteResponse> task = _connect.Executor.Submit<ProcessRouteResponse>(request);
            task.Wait();

            if (task.Exception != null || task.Value == null)
            {
                logger.LogException(task.Exception);
                throw new Exception("Calculate error: " + task.Exception.Message);
            }
            return task.Value.ProcessedRoute;
        }

        /// <summary>
        /// Example how to calculate route by id
        /// </summary>
        /// <param name="route"></param>
        /// <returns>Processed route</returns>
        public ProcessedRoute CalculateRoute(Route route)
        {
            if (route == null)
            {
                throw new Exception("Route not specified");
            }
            var updatedRoute = GetUpdatedRouteById(route.Id);
            ProcessRouteRequest request = new ProcessRouteRequest
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                Route = updatedRoute,
            };

            MessageFuture<ProcessRouteResponse> task = _connect.Executor.Submit<ProcessRouteResponse>(request);
            task.Wait();

            if (task.Exception != null || task.Value == null)
            {
                logger.LogException(task.Exception);
                throw new Exception("Calculate error: " + task.Exception.Message);
            }
            return task.Value.ProcessedRoute;
        }

        /// <summary>
        /// Example hpw to upload processed route to vehicle
        /// </summary>
        /// <param name="route">calculated route</param>
        /// <param name="vehicle">Vehicle where to upload route</param>
        /// <returns></returns>
        public bool UploadRoute(ProcessedRoute route, Vehicle vehicle)
        {
            if (route == null)
            {
                throw new Exception("Route not specified");
            }
            if (vehicle == null)
            {
                throw new Exception("Vehicle not specified");
            }
            UploadRouteRequest request = new UploadRouteRequest
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ProcessedRoute = route,
                Vehicle = vehicle,
            };

            MessageFuture<UploadRouteResponse> task = _connect.Executor.Submit<UploadRouteResponse>(request);
            task.Wait();

            if (task.Exception != null || task.Value == null)
            {
                logger.LogException(task.Exception);
                throw new Exception("Upload error: " + task.Exception.Message);
            }
            return true;
        }
       

    }
}
