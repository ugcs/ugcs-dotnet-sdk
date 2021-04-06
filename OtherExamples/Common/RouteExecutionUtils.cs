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

            var req = new SendCommandRequest
            {
                ClientId = ucs.ClientId,
                Command = new Command
                {
                    Code = "arm",
                    Subsystem = Subsystem.S_FLIGHT_CONTROLLER
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

        public static void Auto(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            var req = new SendCommandRequest
            {
                ClientId = ucs.ClientId,
                Command = new Command
                {
                    Code = "auto",
                    Subsystem = Subsystem.S_FLIGHT_CONTROLLER
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

        public static void Manual(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            var req = new SendCommandRequest
            {
                ClientId = ucs.ClientId,
                Command = new Command
                {
                    Code = "manual",
                    Subsystem = Subsystem.S_FLIGHT_CONTROLLER
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
