using Com.Ugcs.Ucs.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UgCS.SDK.Examples.Common;
using WellKnownTelemetry = UgCS.SDK.Examples.Common.TelemetryUtils.WellKnownTelemetry;

namespace UgCS.SDK.Examples.Takeoff
{
    /// <summary>
    /// This class provide methods to simplify main example code.
    /// </summary>
    internal static class Tools
    {
        /// <summary>
        /// Subscribe to the vehicle telemetry, and wait until the vehicle is armed.
        /// </summary>
        /// <exception cref="TimeoutException">Raised if the vehicle isn't armed during <paramref name="millisecondsTimeout"/></exception>
        public static void WaitUntilArmed(this UcsFacade ucs, Vehicle v, int millisecondsTimeout = 5000)
        {
            using (var syncEvent = new ManualResetEvent(false))
            {
                ucs.SubscribeToVehicleTelemetry(v, e =>
                    {
                        if (e.Telemetry.ToList().Exists(t =>
                                t.TelemetryField.Is(WellKnownTelemetry.IsArmed) &&
                                t.Value != null &&
                                t.Value.HasBoolValue &&
                                t.Value.BoolValue))
                        {
                            syncEvent.Set();
                        }
                    },
                    out SubscriptionToken token);
                bool success = syncEvent.WaitOne(millisecondsTimeout);
                ucs.Unsubscribe(token);
                if (!success)
                    throw new TimeoutException("The vehicle wasn't armed during expected time.");
            }
        }

        /// <summary>
        /// Subscribe to the vehicle telemetry, and wait until the vehicle is armed.
        /// </summary>
        public static bool IsArmed(this UcsFacade ucs, Vehicle v)
        {
            return ucs.GetTelemetry(v).Exists(x =>
                x.TelemetryField.Is(WellKnownTelemetry.IsArmed) &&
                x.Value != null &&
                x.Value.HasBoolValue &&
                x.Value.BoolValue);
        }



        public static bool IsArmSupported(this UcsFacade ucs, Vehicle v)
        {
            return ucs.IsCommandSupported(v, Subsystem.SFlightController, "arm");
        }

        public static bool IsTakeoffSupported(this UcsFacade ucs, Vehicle v)
        {
            return ucs.IsCommandSupported(v, Subsystem.SFlightController, "takeoff_command");
        }

        public static bool IsUploadSupported(this UcsFacade ucs, Vehicle v)
        {
            return ucs.IsCommandSupported(v, Subsystem.SFlightController, "mission_upload");
        }


        private static bool IsCommandSupported(this UcsFacade ucs, Vehicle v, Subsystem subsystem, string commandCode)
        {
            return ucs.GetCommands(v)
                .Exists(c => 
                c.Subsystem == subsystem &&
                c.Code == commandCode && 
                c.HasAvailable &&
                c.Available);
        }

        private static bool IsCommandEnabled(this UcsFacade ucs, Vehicle v, Subsystem subsystem, string commandCode)
        {
            return ucs.GetCommands(v)
                .Exists(c =>
                c.Subsystem == subsystem &&
                c.Code == commandCode &&
                c.HasEnabled &&
                c.Enabled);
        }

        public static bool IsArmEnabled(this UcsFacade ucs, Vehicle v)
        {
            return ucs.IsCommandEnabled(v, Subsystem.SFlightController, "arm");
        }

        public static bool IsTakeoffEnabled(this UcsFacade ucs, Vehicle v)
        {
            return ucs.IsCommandEnabled(v, Subsystem.SFlightController, "takeoff_command");
        }

    }
}
