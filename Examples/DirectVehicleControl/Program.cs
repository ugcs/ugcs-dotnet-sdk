using System;
using System.Collections.Generic;
using System.Threading;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Sdk.Tasks;

namespace DirectVehicleControl
{
    class Program
    {
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

            // Id of the emu-copter is 2
            var vehicleToControl = new Vehicle { Id = 2 };


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
            joystickModeCommand.Vehicles.Add(vehicleToControl);
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
            vehicleJoystickControl.Vehicles.Add(vehicleToControl);

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
                Value = new Value() { DoubleValue = 1 }
            });
            listJoystickCommands.Add(new CommandArgument
            {
                Code = "throttle",
                Value = new Value() { DoubleValue = 1 }
            });

            vehicleJoystickControl.Command.Arguments.AddRange(listJoystickCommands);

            for (int i = 1; i < 11; i++)
            {
                var sendJoystickCommandResponse = messageExecutor.Submit<SendCommandResponse>(vehicleJoystickControl);
                sendJoystickCommandResponse.Wait();
                System.Console.WriteLine("Joystick command to go UP {0}", i);
                Thread.Sleep(1000);
            }


            System.Console.ReadKey();

            tcpClient.Close();
            messageSender.Cancel();
            messageReceiver.Cancel();
            messageExecutor.Close();
            notificationListener.Dispose();
        }
    }
}
