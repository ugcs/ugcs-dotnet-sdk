using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using static UgCS.SDK.Examples.Common.TelemetryUtils;

namespace UgCS.SDK.Examples.Common
{
    public static class VehicleUtils
    {
        public delegate void CommandEventHandler(CommandEvent e);
        public delegate void TelemetryEventHandler(TelemetryEvent e);



        public static void SubscribeToVehicleCommands(this UcsFacade ucs, Vehicle v,
            CommandEventHandler handler, out SubscriptionToken token)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            ucs.Subscribe(
                new EventSubscriptionWrapper
                {
                    CommandSubscription = new CommandSubscription
                    {
                        Vehicle = v
                    }
                },
                (e) => handler(e.Event.CommandEvent),
                out token);
        }

        public static void SubscribeToVehicleTelemetry(this UcsFacade ucs, Vehicle v,
            TelemetryEventHandler handler, out SubscriptionToken token)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            ucs.Subscribe(
                new EventSubscriptionWrapper
                {
                    TelemetrySubscription = new TelemetrySubscription
                    {
                        Vehicle = v
                    }
                },
                (e) => handler(e.Event.TelemetryEvent),
                out token);
        }

        public static void AcquireLock(this UcsFacade ucs, Vehicle v)
        {
            if (ucs == null)
                throw new ArgumentNullException(nameof(ucs));
            if (v == null)
                throw new ArgumentNullException(nameof(v));


            ucs.Execute<AcquireLockResponse>(
                new AcquireLockRequest
                {
                    ClientId = ucs.ClientId,
                    ObjectType = "Vehicle",
                    ObjectId = v.Id
                });
        }

        public static List<CommandDefinition> GetCommands(this UcsFacade ucs, Vehicle v, int millisecondsTimeout = 1000)
        {
            using (var syncEvent = new ManualResetEvent(false))
            {
                var supportedCommands = new List<CommandDefinition>();
                ucs.SubscribeToVehicleCommands(v, e =>
                {
                    supportedCommands.AddRange(e.Commands);
                    syncEvent.Set();
                },
                    out SubscriptionToken token);
                bool success = syncEvent.WaitOne(millisecondsTimeout);
                ucs.Unsubscribe(token);
                if (!success)
                    throw new TimeoutException("The UCS didn't send a command event during expected time.");
                return supportedCommands;
            }
        }

        public static bool IsEmulator(this Vehicle v)
        {
            return !v.IsReal;
        }


        /// <summary>
        /// Subscribe to the vehicle telemetry, and wait until the vehicle is armed.
        /// </summary>
        public static bool IsOnline(this UcsFacade ucs, Vehicle v)
        {
            Telemetry? t = ucs.GetTelemetry(v, WellKnownTelemetry.DownlinkPresent, millisecondsTimeout: 1000);
            return
                t != null &&
                t.Value != null &&
                t.Value.BoolValueSpecified &&
                t.Value.BoolValue;
        }
    }
}
