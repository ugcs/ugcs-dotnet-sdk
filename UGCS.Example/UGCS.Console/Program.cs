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
using System.Threading;

namespace UGCS.Console
{

    class Program
    {
        private static object getTelemetryValue(Value telemetryValue)
        {
            if (telemetryValue == null)
            {
                return null;
            }
            if (telemetryValue.IntValueSpecified)
            {
                return telemetryValue.IntValue;
            }
            if (telemetryValue.LongValueSpecified)
            {
                return telemetryValue.LongValue;
            }
            if (telemetryValue.StringValueSpecified)
            {
                return telemetryValue.StringValue;
            }
            if (telemetryValue.BoolValueSpecified)
            {
                return telemetryValue.BoolValue;
            }
            if (telemetryValue.DoubleValueSpecified)
            {
                return telemetryValue.DoubleValue;
            }
            if (telemetryValue.FloatValueSpecified)
            {
                return telemetryValue.FloatValue;
            }
            return null;
        }

        static void Main(string[] args)
        {

            //Connect
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("localhost", 3334);
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

            //Lock vehicle example
            AcquireLockRequest lockRequest = new AcquireLockRequest
            {
                ClientId = clientId,
                ObjectType = "Vehicle",
                ObjectId = 2
            };

            var resultLock = messageExecutor.Submit<AcquireLockResponse>(lockRequest);
            resultLock.Wait();

            // Click&Go example

            var sendCommandRequestGuided = new SendCommandRequest
            {
                ClientId = clientId,
                Command = new UGCS.Sdk.Protocol.Encoding.Command
                {
                    Code = "guided",
                    Subsystem = Subsystem.S_FLIGHT_CONTROLLER
                }
            };
            sendCommandRequestGuided.Vehicles.Add(new Vehicle { Id = 2 });
            var sendCommandResponseGuided = messageExecutor.Submit<SendCommandResponse>(sendCommandRequestGuided);
            sendCommandResponseGuided.Wait();

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

            //get emucopter route
            GetObjectRequest getObjectRequest = new GetObjectRequest()
            {
                ClientId = clientId,
                ObjectType = "Route",
                RefreshDependencies = true,
                ObjectId = 1
            };
            var taskRoute = messageExecutor.Submit<GetObjectResponse>(getObjectRequest);
            taskRoute.Wait();
            var routeFromServer = taskRoute.Value;

            //Get all vehicles
            GetObjectListRequest getObjectListRequest = new GetObjectListRequest()
            {
                ClientId = clientId,
                ObjectType = "Vehicle",
                RefreshDependencies = true
            };
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

            //vehicles in mission
            System.Console.WriteLine("Vehicles in mission:");
            foreach (var vehicleMission in missionFromUcs.Vehicles)
            {
                System.Console.WriteLine(vehicleMission.Vehicle.Name);
            }
            missionFromUcs.Vehicles.Clear();

            //Add vehicle to mission
            missionFromUcs.Vehicles.Add(
                  new MissionVehicle
                  {
                      Vehicle = vehicle
                  });

            System.Console.WriteLine("Vehicles in mission after add vehicle:");
            foreach (var vehicleMission in missionFromUcs.Vehicles)
            {
                System.Console.WriteLine(vehicleMission.Vehicle.Name);
            }

            //save mission
            CreateOrUpdateObjectRequest createOrUpdateObjectRequestForMission = new CreateOrUpdateObjectRequest()
            {
                ClientId = clientId,
                Object = new DomainObjectWrapper().Put(missionFromUcs, "Mission"),
                WithComposites = true,
                ObjectType = "Mission",
                AcquireLock = false
            };
            var updateMissionTask = messageExecutor.Submit<CreateOrUpdateObjectResponse>(createOrUpdateObjectRequestForMission);
            updateMissionTask.Wait();

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
            
            //add action to route
            ActionDefinition actionDefenition = new ActionDefinition();
            actionDefenition.HeadingDefinition = new HeadingDefinition();
            actionDefenition.HeadingDefinition.Heading = 1.57079633; // 90 degrees
            actionDefenition.HeadingDefinition.RelativeToNorth = true; 
            if (routeFromUcs.Segments.Count > 2)
            {
                routeFromUcs.Segments[1].ActionDefinitions.Add(actionDefenition);
            }
            System.Console.WriteLine(string.Format("action to route '{0}'", routeFromUcs.Name));

            //save route
            CreateOrUpdateObjectRequest createOrUpdateRouteRequest = new CreateOrUpdateObjectRequest()
            {
                ClientId = clientId,
                Object = new DomainObjectWrapper().Put(routeFromUcs, "Route"),
                WithComposites = true,
                ObjectType = "Route",
                AcquireLock = false
            };
            var createOrUpdateRouteResponseTask = messageExecutor.Submit<CreateOrUpdateObjectResponse>(createOrUpdateRouteRequest);
            createOrUpdateRouteResponseTask.Wait();
            if (createOrUpdateRouteResponseTask.Value != null)
            {
                System.Console.WriteLine(string.Format("'{0}' route updated on UCS", routeFromUcs.Name));
            }
            else
            {
                System.Console.WriteLine(string.Format("fail to update route '{0}' on UCS", routeFromUcs.Name));
            }

            // Payload control
            SendCommandRequest requestPaload = new SendCommandRequest
            {
                ClientId = clientId,
                Command = new UGCS.Sdk.Protocol.Encoding.Command
                {
                    Code = "direct_payload_control",
                    Subsystem = Subsystem.S_GIMBAL,
                    Silent = true,
                    ResultIndifferent = true
                }
            };
            requestPaload.Vehicles.Add(new Vehicle() { Id = vehicle.Id });

            List<CommandArgument> listCommands = new List<CommandArgument>();
            listCommands.Add(new CommandArgument
            {
                Code = "roll",
                Value = new Value() { DoubleValue = 1 }
            });
            listCommands.Add(new CommandArgument
            {
                Code = "pitch",
                Value = new Value() { DoubleValue = 0 }
            });
            listCommands.Add(new CommandArgument
            {
                Code = "yaw",
                Value = new Value() { DoubleValue = 0 }
            });
            listCommands.Add(new CommandArgument
            {
                Code = "zoom",
                Value = new Value() { DoubleValue = 0 }
            });

            requestPaload.Command.Arguments.AddRange(listCommands);

            var resultPayload = messageExecutor.Submit<SendCommandResponse>(requestPaload);
            resultPayload.Wait();

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

            // Get Telemetry for vehicle
            DateTime utcTime = DateTime.Now.ToUniversalTime();
            DateTime posixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan span = utcTime - posixEpoch;
            var beginningMilliseconds = (long)span.TotalMilliseconds;
            GetTelemetryRequest telemetryRequest = new GetTelemetryRequest
            {
                ClientId = clientId,
                FromTime = beginningMilliseconds,
                Limit = 10,
                Vehicle = new Vehicle() { Id = 1 }
            };
            var responseTelemetry = messageExecutor.Submit<GetTelemetryResponse>(telemetryRequest);
            responseTelemetry.Wait();

            //Go to manual mode
            SendCommandRequest manualModeCommand = new SendCommandRequest
            {
                ClientId = clientId,
                Command = new UGCS.Sdk.Protocol.Encoding.Command
                {
                    Code = "manual",
                    Subsystem = Subsystem.S_FLIGHT_CONTROLLER,
                    Silent = false,
                    ResultIndifferent = false
                }
            };
            manualModeCommand.Vehicles.Add(new Vehicle() { Id = 2 });
            var manualMode = messageExecutor.Submit<SendCommandResponse>(manualModeCommand);
            manualMode.Wait();

            //Go to joystick mode
            SendCommandRequest joystickModeCommand = new SendCommandRequest
            {
                ClientId = clientId,
                Command = new UGCS.Sdk.Protocol.Encoding.Command
                {
                    Code = "joystick",
                    Subsystem = Subsystem.S_FLIGHT_CONTROLLER,
                    Silent = false,
                    ResultIndifferent = false
                }
            };
            joystickModeCommand.Vehicles.Add(new Vehicle() { Id = 2 });
            var joystickMode = messageExecutor.Submit<SendCommandResponse>(joystickModeCommand);
            joystickMode.Wait();

            // Vehicle control in joystick mode
            SendCommandRequest vehicleJoystickControl = new SendCommandRequest
            {
                ClientId = clientId,
                Command = new UGCS.Sdk.Protocol.Encoding.Command
                {
                    Code = "direct_vehicle_control",
                    Subsystem = Subsystem.S_FLIGHT_CONTROLLER,
                    Silent = true,
                    ResultIndifferent = true
                }
            };
            vehicleJoystickControl.Vehicles.Add(new Vehicle() { Id = 2 });

            //List of current joystick values to send to vehicle.
            List<CommandArgument> listJoystickCommands = new List<CommandArgument>();
            listJoystickCommands.Add(new CommandArgument
            {
                Code = "roll",
                Value = new Value() { DoubleValue = 0 }
            });
            listJoystickCommands.Add(new CommandArgument
            {
                Code = "pitch",
                Value = new Value() { DoubleValue = 0 }
            });
            listJoystickCommands.Add(new CommandArgument
            {
                Code = "yaw",
                Value = new Value() { DoubleValue = 0 }
            });
            listJoystickCommands.Add(new CommandArgument
            {
                Code = "throttle",
                Value = new Value() { DoubleValue = 1 }
            });

            vehicleJoystickControl.Command.Arguments.AddRange(listJoystickCommands);

            for (int i = 1; i < 11; i++ )
            {
                var sendJoystickCommandResponse = messageExecutor.Submit<SendCommandResponse>(vehicleJoystickControl);
                resultPayload.Wait();
                System.Console.WriteLine("Joystick command to go UP {0}", i);
                Thread.Sleep(1000);
            }

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
                    foreach (Telemetry t in notification.Event.TelemetryEvent.Telemetry)
                    {
                        System.Console.WriteLine("Vehicle id: {0} Code: {1} Semantic {2} Subsystem {3} Value {4}", notification.Event.TelemetryEvent.Vehicle.Id, t.TelemetryField.Code, t.TelemetryField.Semantic, t.TelemetryField.Subsystem, getTelemetryValue(t.Value));
                    }
                }
            ), telemetrySubscriptionWrapper);
            notificationListener.AddSubscription(stTelemetry);

            //Log notification subscription
            var logSubscriptionWrapper = new EventSubscriptionWrapper();
            logSubscriptionWrapper.ObjectModificationSubscription = new ObjectModificationSubscription();
            logSubscriptionWrapper.ObjectModificationSubscription.ObjectType = "VehicleLogEntry";
            SubscribeEventRequest requestLogEvent = new SubscribeEventRequest();
            requestLogEvent.ClientId = clientId;
            requestLogEvent.Subscription = logSubscriptionWrapper;
            var responceLog = messageExecutor.Submit<SubscribeEventResponse>(requestLogEvent);
            var subscribeEventResponseLog = responceLog.Value;

            SubscriptionToken stLog = new SubscriptionToken(subscribeEventResponseLog.SubscriptionId, (
                (notification) =>
                {
                    var eventType = notification.Event.ObjectModificationEvent.ModificationType;
                    var eventLog = notification.Event.ObjectModificationEvent.Object.VehicleLogEntry;
                    if (eventType == ModificationType.MT_CREATE)
                    {
                        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        DateTime date = start.AddMilliseconds(eventLog.Time).ToLocalTime();
                        var command = eventLog.CommandArguments != null ? eventLog.CommandArguments.CommandCode : string.Empty;
                        System.Console.WriteLine("LOG: {0} Vehicle id: {1} Command: {2} Message: {3}", date.ToString("HH:mm:ss"), eventLog.Vehicle.Id, command, eventLog.Message);
                    }

                }), logSubscriptionWrapper);
            notificationListener.AddSubscription(stLog);

            //Object notification subscription, subscribe for mission changed 
            var missionObjectSubscriptionWrapper = new EventSubscriptionWrapper();
            missionObjectSubscriptionWrapper.ObjectModificationSubscription = new ObjectModificationSubscription();
            missionObjectSubscriptionWrapper.ObjectModificationSubscription.ObjectType = "Mission";
            SubscribeEventRequest requestMissionEvent = new SubscribeEventRequest();
            requestMissionEvent.ClientId = clientId;
            requestMissionEvent.Subscription = missionObjectSubscriptionWrapper;
            var responceMission = messageExecutor.Submit<SubscribeEventResponse>(requestMissionEvent);
            var subscribeEventResponseMission = responceMission.Value;

            SubscriptionToken stMission = new SubscriptionToken(subscribeEventResponseMission.SubscriptionId, (
                (notification) =>
                {
                    var eventType = notification.Event.ObjectModificationEvent.ModificationType;
                    var missionObject = notification.Event.ObjectModificationEvent.Object.Mission;
                    if (eventType == ModificationType.MT_UPDATE)
                    {
                        System.Console.WriteLine("Mission id: {0} updated", missionObject.Id);
                    }

                }), missionObjectSubscriptionWrapper);
            notificationListener.AddSubscription(stMission);


            System.Console.ReadKey();

            tcpClient.Close();
            messageSender.Cancel();
            messageReceiver.Cancel();
            messageExecutor.Close();
            notificationListener.Dispose();

        }
    }
}
