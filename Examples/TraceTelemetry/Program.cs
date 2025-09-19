
using UgCS.SDK.Examples.Common;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Com.Ugcs.Ucs.Proto;

object telemetrySync = new();
var jsonOptions = new JsonSerializerOptions { WriteIndented = true };

string serialNumber = String.Join(' ', args).Trim();
if (String.IsNullOrWhiteSpace(serialNumber))
{
    i("This application print all telemetry messages received from the server for the vehicle " +
        "with the provided serial number." + Environment.NewLine + Environment.NewLine +
        "Usage: trace-telemetry.exe <aircraft_serial_number>");
    Environment.Exit(2);
    return;
}

try
{
    using (UcsFacade ucs = UcsFacade.connectToUcs("localhost"))
    {
        i($"Connected to ucs. Listening for telemetry from the vehicle with serial number '{serialNumber}'.");
        var disconnectionLock = new ManualResetEvent(false);
        ucs.Disconnected += new EventHandler((_, _) => {
            disconnectionLock.Set();
        });


        Vehicle vehicle = ucs.List<Vehicle>().SingleOrDefault(it => String.Equals(it.SerialNumber, serialNumber,  StringComparison.InvariantCultureIgnoreCase))
            ?? throw new InvalidOperationException($"Vehicle with serial number '{serialNumber}' is not registered on the server.");

        ucs.SubscribeToVehicleTelemetry(vehicle, onTelemetry, out SubscriptionToken token);

        var waitingForQ = Task.Run(() =>
        {
            ConsoleKeyInfo? pressedKey = Console.ReadKey();
            while (pressedKey?.Key != ConsoleKey.Q)
            {
                i("Press 'Q' to exit.");
                pressedKey = Console.ReadKey();
            }
        });

        var waitingForDisconnection = Task.Run(() =>
        {
            disconnectionLock.WaitOne();
            i("Closing as connection with UCS lost.");
            Environment.Exit(1);
        });


        Task.WaitAny(waitingForQ, waitingForDisconnection);
    }
}
catch (Exception ex)
{
    e(ex);
    Environment.Exit(1);
}


void onTelemetry(TelemetryEvent e)
{
    lock (telemetrySync)
    {
        i(JsonSerializer.Serialize(e.Telemetry, jsonOptions));
        /*
        i($"Received {e.Telemetry.Count} telemetry values.");
        foreach (var v in e.Telemetry)
        {
            i($"'{v.TelemetryField.Code}'='{getValue(v.Value)}'");
        }
        */
    }
}

object? getValue(Value? value)
{
    if (value == null)
        return null;
    else if (value.HasStringValue)
        return value.StringValue;
    else if (value.HasDoubleValue)
        return value.DoubleValue;
    if (value.HasFloatValue)
        return value.FloatValue;
    else if (value.HasLongValue)
        return value.LongValue;
    else if (value.HasIntValue)
        return value.IntValue;
    else
        return null;
}

void i(String msg)
{
    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} I/app : {msg}");
}

void e(Exception e)
{
    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} E/app : {e.Message}");
}