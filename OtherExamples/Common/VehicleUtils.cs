using System;
using System.Collections.Generic;
using System.Text;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

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
    }
}
