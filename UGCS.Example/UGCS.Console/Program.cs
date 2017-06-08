using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Sdk.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.IO;

namespace UGCS.Console
{

    class Program
    {
        static void Main(string[] args)
        {

            //Connect
            TcpClient tcpClient = new TcpClient("localhost", 3334);
            MessageSender messageSender = new MessageSender(tcpClient.Session);
            MessageReceiver messageReceiver = new MessageReceiver(tcpClient.Session);
            MessageExecutor messageExecutor = new MessageExecutor(messageSender, messageReceiver, new InstantTaskScheduler());
            messageExecutor.Configuration.DefaultTimeout = 10000;
            var notificationListener = new NotificationListener();
            messageReceiver.AddListener(-1, notificationListener);

            //auth
            AuthorizeHciRequest request = new AuthorizeHciRequest();
            request.ClientId = -1;
            request.Locale = "en-US";
            var future = messageExecutor.Submit<AuthorizeHciResponse>(request);
            future.Wait();
            AuthorizeHciResponse AuthorizeHciResponse = future.Value;
            int clientId = AuthorizeHciResponse.ClientId;
            System.Console.WriteLine("AuthorizeHciResponse precessed");

            //login
            LoginRequest loginRequest = new LoginRequest();
            loginRequest.UserLogin = "admin";
            loginRequest.UserPassword = "admin";
            loginRequest.ClientId = clientId;
            var loginResponcetask = messageExecutor.Submit<LoginResponse>(loginRequest);
            loginResponcetask.Wait();


            // Click&Go example
            var sendCommandRequest = new SendCommandRequest
            {
                ClientId = clientId,
                Command = new UGCS.Sdk.Protocol.Encoding.Command
                {
                    Code = "waypoint",
                    Subsystem = Subsystem.S_FLIGHT_CONTROLLER
                }
            };

            sendCommandRequest.Command.Arguments.AddRange(new CommandArgument[] {
									new CommandArgument { Code = "latitude", Value = new Value { DoubleValue = 0.994445232147517 }},
									new CommandArgument { Code = "longitude", Value = new Value { DoubleValue = 0.4201742565140717 }},
									new CommandArgument { Code = "altitude_agl", Value = new Value { DoubleValue = 5.0 }},
									new CommandArgument { Code = "ground_speed", Value = new Value { DoubleValue = 5.0 }},
									new CommandArgument { Code = "heading", Value = new Value { DoubleValue = 0.017453292519943295 }}
								});

            sendCommandRequest.Vehicles.Add(new Vehicle { Id = 2 });

            var sendCommandResponse = messageExecutor.Submit<SendCommandResponse>(sendCommandRequest);
            sendCommandResponse.Wait();
            System.Console.WriteLine("Click&Go command sent");


            //Import mission
            var byteArray = File.ReadAllBytes("Demo mission.xml");
            ImportMissionFromXmlRequest importMissionRequest = new ImportMissionFromXmlRequest()
            {
                ClientId = clientId,
                MissionXml = byteArray
            };
            var importMissionResponse = messageExecutor.Submit<ImportMissionFromXmlResponse>(importMissionRequest);
            importMissionResponse.Wait();
            //mission contains imported mission from Demo mission.xml
            var mission = importMissionResponse.Value.Mission;
            System.Console.WriteLine("Demo mission.xml imported to UCS with name '{0}'", mission.Name);

            //Get mission from server
            GetObjectRequest getMissionObjectRequest = new GetObjectRequest()
            {
                ClientId = clientId,
                ObjectType = "Mission",
                ObjectId = mission.Id,
                RefreshDependencies = true
            };
            var getMissionObjectResponse = messageExecutor.Submit<GetObjectResponse>(getMissionObjectRequest);
            getMissionObjectResponse.Wait();
            //missionFromUcs contains retrieved mission
            var missionFromUcs = getMissionObjectResponse.Value.Object.Mission;
            System.Console.WriteLine("mission id '{0}' retrieved from UCS with name '{1}'", mission.Id, missionFromUcs.Name);

            //Import route
            var byteArrayRoute = File.ReadAllBytes("Demo route for Copter.xml");
            ImportRouteRequest importRouteRequest = new ImportRouteRequest()
            {
                ClientId = clientId,
                RouteData = byteArrayRoute,
                Filename = "Demo route for Copter.xml"
            };
            var importRouteResponse = messageExecutor.Submit<ImportRouteResponse>(importRouteRequest);
            importRouteResponse.Wait();
            //importedRoute contains imported route from Demo route for Copter.xml
            var importedRoute = importRouteResponse.Value.Route;
            System.Console.WriteLine("Demo route for Copter.xml imported to UCS with name '{0}'", importedRoute.Name);
            //Add vehicle profile to route
            GetObjectRequest requestVehicle = new GetObjectRequest()
            {
                ClientId = clientId,
                ObjectType = "Vehicle",
                ObjectId = 1, //EMU-COPTER-17
                RefreshDependencies = true
            };
            var responseVehicle = messageExecutor.Submit<GetObjectResponse>(requestVehicle);
            responseVehicle.Wait();
            importedRoute.VehicleProfile = responseVehicle.Value.Object.Vehicle.Profile;
            //Add route to mission
            importedRoute.Mission = missionFromUcs;
            //Save route on server
            CreateOrUpdateObjectRequest routeSaveRequest = new CreateOrUpdateObjectRequest()
            {
                ClientId = clientId,
                Object = new DomainObjectWrapper().Put(importedRoute, "Route"),
                WithComposites = true,
                ObjectType = "Route",
                AcquireLock = false
            };
            var updateRouteTask = messageExecutor.Submit<CreateOrUpdateObjectResponse>(routeSaveRequest);
            updateRouteTask.Wait();
            System.Console.WriteLine("route '{0}' added to mission '{1}'", updateRouteTask.Value.Object.Route.Name, missionFromUcs.Name);
            
            //Get route from server
            GetObjectRequest getRouteObjectRequest = new GetObjectRequest()
            {
                ClientId = clientId,
                ObjectType = "Route",
                ObjectId = updateRouteTask.Value.Object.Route.Id,
                RefreshDependencies = true
            };
            var geRouteObjectResponse = messageExecutor.Submit<GetObjectResponse>(getRouteObjectRequest);
            geRouteObjectResponse.Wait();
            //routeFromUcs contains retrieved route
            var routeFromUcs = geRouteObjectResponse.Value.Object.Route;
            System.Console.WriteLine(string.Format("route id '{0}' retrieved from UCS with name '{1}'", updateRouteTask.Value.Object.Route.Id, routeFromUcs.Name));

            //Get all vehicles
            GetObjectListRequest getObjectListRequest = new GetObjectListRequest()
            {
                ClientId = clientId,
                ObjectType = "Vehicle",
                RefreshDependencies = true
            };
            getObjectListRequest.RefreshExcludes.Add("Avatar");
            getObjectListRequest.RefreshExcludes.Add("PayloadProfile");
            getObjectListRequest.RefreshExcludes.Add("Route");
            var task = messageExecutor.Submit<GetObjectListResponse>(getObjectListRequest);
            task.Wait();

            var list = task.Value;
            foreach (var v in list.Objects)
            {
                System.Console.WriteLine(string.Format("name: {0}; id: {1}; type: {2}",
                       v.Vehicle.Name, v.Vehicle.Id, v.Vehicle.Type.ToString()));
            }
            Vehicle vehicle = task.Value.Objects.FirstOrDefault().Vehicle;

            //update vehicle object
            CreateOrUpdateObjectRequest createOrUpdateObjectRequest = new CreateOrUpdateObjectRequest()
            {
                ClientId = clientId,
                Object = new DomainObjectWrapper().Put(vehicle, "Vehicle"),
                WithComposites = true,
                ObjectType = "Vehicle",
                AcquireLock = false
            };
            var createOrUpdateObjectResponseTask = messageExecutor.Submit<CreateOrUpdateObjectResponse>(createOrUpdateObjectRequest);
            createOrUpdateObjectResponseTask.Wait();

            //Vehicle notification subscription
            var eventSubscriptionWrapper = new EventSubscriptionWrapper();
            eventSubscriptionWrapper.ObjectModificationSubscription = new ObjectModificationSubscription();
            eventSubscriptionWrapper.ObjectModificationSubscription.ObjectId = vehicle.Id;
            eventSubscriptionWrapper.ObjectModificationSubscription.ObjectType = "Vehicle";
            SubscribeEventRequest requestEvent = new SubscribeEventRequest();
            requestEvent.ClientId = clientId;
            requestEvent.Subscription = eventSubscriptionWrapper;
            var responce = messageExecutor.Submit<SubscribeEventResponse>(requestEvent);
            responce.Wait();
            var subscribeEventResponse = responce.Value;
            SubscriptionToken st = new SubscriptionToken(subscribeEventResponse.SubscriptionId, (
                (notification) =>
                {
                    //Vehicle notification
                }
            ), eventSubscriptionWrapper);
            notificationListener.AddSubscription(st);

            //TelemetrySubscription
            var telemetrySubscriptionWrapper = new EventSubscriptionWrapper();
            telemetrySubscriptionWrapper.TelemetrySubscription = new TelemetrySubscription();
            SubscribeEventRequest requestTelemetryEvent = new SubscribeEventRequest();
            requestTelemetryEvent.ClientId = clientId;
            requestTelemetryEvent.Subscription = telemetrySubscriptionWrapper;
            var responceTelemetry = messageExecutor.Submit<SubscribeEventResponse>(requestTelemetryEvent);
            responceTelemetry.Wait();
            var subscribeEventResponseTelemetry = responceTelemetry.Value;
            SubscriptionToken stTelemetry = new SubscriptionToken(subscribeEventResponseTelemetry.SubscriptionId, (
                (notification) =>
                {
                    foreach (var t in notification.Event.TelemetryEvent.Telemetry)
                    {
                        System.Console.WriteLine("Vehicle id: {0} Type: {1} Value {2}", t.Vehicle.Id, t.Type.ToString(), t.Value);
                    }
                }
            ), telemetrySubscriptionWrapper);
            notificationListener.AddSubscription(stTelemetry);



            System.Console.ReadKey();

            tcpClient.Close();
            messageSender.Cancel();
            messageReceiver.Cancel();
            messageExecutor.Close();
            notificationListener.Dispose();

        }
    }
}
