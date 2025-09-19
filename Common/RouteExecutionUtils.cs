using Com.Ugcs.Ucs.Proto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

            ucs.Execute<UploadRouteRequest, UploadRouteResponse>(
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

            ucs.executeCommand(v, Subsystem.SFlightController, "arm");
        }
        
        public static void Takeoff(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            ucs.executeCommand(v, Subsystem.SFlightController, "takeoff_command");
        }
        
        public static void Land(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            ucs.executeCommand(v, Subsystem.SFlightController, "land_command");
        }        

        public static void Auto(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            ucs.executeCommand(v, Subsystem.SFlightController, "auto");
        }

        public static void Manual(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            ucs.executeCommand(v, Subsystem.SFlightController, "manual");
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

            var resp = ucs.Execute<SendCommandRequest, SendCommandResponse>(req);

            Debug.Assert(resp.CommandResults.Count == 1);
            VehicleCommandResultDto executionResult = resp.CommandResults[0];

            Debug.Assert(executionResult != null);
            if (executionResult.CommandStatus != CommandStatus.CsSucceeded)
                throw new ApplicationException($"Command execution failed: '{executionResult.ErrorMessage}'.");
        }
    }
}
