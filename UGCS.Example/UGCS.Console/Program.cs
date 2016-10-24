using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Sdk.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace UGCS.Console
{
    public class TelemetryListenerTest : IMessageListener
    {
        public void MessageReceived(IExtensible message)
        {
            int x;
            var msg = (Notification)message;
            String s = "";
            foreach (Telemetry t in msg.Event.TelemetryEvent.Telemetry)
            {
                if (t.Vehicle.Id != 1)
                {
                    continue;
                }
                System.Console.WriteLine(t.Vehicle.Id + ": " + t.Type.ToString() + " - "+ t.Value);
                switch (t.Type)
                {
                    case TelemetryType.TT_LATITUDE:
                        x = 1;
                        break;
                    case TelemetryType.TT_LONGITUDE:
                        x = 1;
                        break;
                    case TelemetryType.TT_LATITUDE_GPS:
                        x = 1;
                        break;
                    case TelemetryType.TT_LONGITUDE_GPS:
                        x = 1;
                        break;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            var Listener = new TelemetryListenerTest();
            EventSubscriptionWrapper esw = new EventSubscriptionWrapper();
            esw.TelemetrySubscription = new TelemetrySubscription();

            TcpClient tcpClient = new TcpClient("localhost", 3334);
            MessageSender messageSender = new MessageSender(tcpClient.Session);
            MessageReceiver messageReceiver = new MessageReceiver(tcpClient.Session);
            messageReceiver.AddListener(-1, Listener);
            MessageExecutor messageExecutor = new MessageExecutor(messageSender, messageReceiver, new InstantTaskScheduler());
            messageExecutor.Configuration.DefaultTimeout = 10000;
            AuthorizeHciRequest request = new AuthorizeHciRequest();
            request.ClientId = -1;
            LoginRequest loginRequest = new LoginRequest();
            loginRequest.UserLogin = "admin";
            loginRequest.UserPassword = "admin";

            var future = messageExecutor.Submit<AuthorizeHciResponse>(request);
            future.Wait();
            AuthorizeHciResponse AuthorizeHciResponse = future.Value;

            loginRequest.ClientId = AuthorizeHciResponse.ClientId;
            LoginResponse LoginResponce = messageExecutor.Submit<LoginResponse>(loginRequest).Value;


            System.Console.WriteLine("Telemetry subscribed");

            GetObjectRequest GetObjectRequest = new GetObjectRequest()
            {
                ClientId = AuthorizeHciResponse.ClientId,
                ObjectId = 1,
                ObjectType = "Route",
                RefreshDependencies = true
            };

            GetObjectRequest.RefreshExcludes.Add("Avatar");
            GetObjectRequest.RefreshExcludes.Add("Vehicle");
            GetObjectRequest.RefreshExcludes.Add("PayloadProfile");
            GetObjectRequest.RefreshExcludes.Add("Mission");

            var task = messageExecutor.Submit<GetObjectResponse>(GetObjectRequest);
            task.Wait();

            Route route = task.Value.Object.Route;

            ProcessRouteRequest ProcessRouteRequest = new ProcessRouteRequest
            {
                ClientId = AuthorizeHciResponse.ClientId,
                Route = route,
            };

            MessageFuture<ProcessRouteResponse> ProcessRouteResponse = messageExecutor.Submit<ProcessRouteResponse>(ProcessRouteRequest);
            ProcessRouteResponse.Wait();

            if (ProcessRouteResponse.Exception != null || ProcessRouteResponse.Value == null)
            {
                throw new Exception("Calculate error: " + ProcessRouteResponse.Exception.Message);
            }

            System.Console.WriteLine("Route calculated");


            SubscribeEventRequest requestEvent = new SubscribeEventRequest();
            requestEvent.ClientId = AuthorizeHciResponse.ClientId;

            requestEvent.Subscription = esw;
            var tm = messageExecutor.Submit<SubscribeEventResponse>(requestEvent);
            tm.Wait();

            System.Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
