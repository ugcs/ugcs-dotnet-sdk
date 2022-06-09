// Connectung to UgCS Server (UCS).
using UgCS.SDK.Examples.Common;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

using (UcsFacade ucs = UcsFacade.connectToUcs("localhost"))
{
    Console.WriteLine("Connected to ucs.");


    ucs.SubscribeToObjectModification(
        typeof(VehicleLogEntry).Name,
        (args) => {
            if (args.ModificationTypeSpecified && args.ModificationType == ModificationType.MT_CREATE)
                print(args.Object.VehicleLogEntry);
        },
        out SubscriptionToken token);


    Console.WriteLine("Listening for messages from all vehicles. Press any key to exit.");
    Console.ReadKey();
}

void print(VehicleLogEntry entry)
{
    Console.WriteLine("[{0}][{1}] {2}",
        entry.Vehicle.Name,
        entry.LevelSpecified ? entry.Level : "UNKNOWN",
        entry.Message);
}