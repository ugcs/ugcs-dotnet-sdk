using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UGCS.Sdk.Protocol.Encoding;

namespace UgCS.SDK.Examples.Common
{
    public static class RouteExecutionUtils
    {
        public static void Upload(this UcsFacade ucs, Vehicle v, ProcessedRoute r, int startIndex = 0)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));
            if (r == null)
                throw new ArgumentNullException(nameof(r));

            ucs.Execute<UploadRouteResponse>(
                new UploadRouteRequest
                {
                    ClientId = ucs.ClientId,
                    Vehicle = v,
                    ProcessedRoute = r,
                    StartIndex = startIndex,
                    AddFirstWaypoint = false
                });
        }

        public static void Arm(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            ucs.executeCommand(v, Subsystem.S_FLIGHT_CONTROLLER, "arm");
        }
        
        public static void Takeoff(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            ucs.executeCommand(v, Subsystem.S_FLIGHT_CONTROLLER, "takeoff_command");
        }
        
        public static void Land(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            ucs.executeCommand(v, Subsystem.S_FLIGHT_CONTROLLER, "land_command");
        }        

        public static void Auto(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            ucs.executeCommand(v, Subsystem.S_FLIGHT_CONTROLLER, "auto");
        }

        public static void Manual(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            ucs.executeCommand(v, Subsystem.S_FLIGHT_CONTROLLER, "manual");
        }

        private static void executeCommand(this UcsFacade ucs, Vehicle v,
            Subsystem subsystem, string commandCode)
        {
            var req = new SendCommandRequest
            {
                ClientId = ucs.ClientId,
                Command = new Command
                {
                    Code = commandCode,
                    Subsystem = subsystem
                }
            };
            req.Vehicles.Add(v);

            var resp = ucs.Execute<SendCommandResponse>(req);

            Debug.Assert(resp.CommandResults.Count == 1);
            VehicleCommandResultDto executionResult = resp.CommandResults[0];

            Debug.Assert(executionResult != null);
            if (executionResult.CommandStatus != CommandStatus.CS_SUCCEEDED)
                throw new ApplicationException($"Command execution failed: '{executionResult.ErrorMessage}'.");
        }
    }
}
