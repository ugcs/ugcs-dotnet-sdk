// Connectung to UgCS Server (UCS).
using Com.Ugcs.Ucs.Proto;
using System;
using System.Security.Claims;
using UgCS.SDK.Examples.Common;

object telemetrySync = new();


using (UcsFacade ucs = UcsFacade.connectToUcs("localhost"))
{
    Console.WriteLine("Connected to ucs.");

    try
    {
        Vehicle? vehicle;
        do
        {
            vehicle = ucs.List<Vehicle>().SingleOrDefault(it => it.SerialNumber == "test");
            if (vehicle == null)
            {
                Console.WriteLine("Vehicle 'test' is not connected. Will retry 5 seconds later...");
                Thread.Sleep(5000);
            }
        } while (vehicle == null);

        ucs.SubscribeToVehicleTelemetry(vehicle, onTelemetry, out SubscriptionToken token);

        Console.WriteLine("Listening for messages from all vehicles. Press any key to exit.");
        Console.ReadKey();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{ex.Message}");
    }
}


void onTelemetry(TelemetryEvent e)
{
    lock (telemetrySync)
    {
        i($"Received {e.Telemetry.Count} telemetry values.");
        foreach (var v in e.Telemetry)
        {
            i($"'{v.TelemetryField.Code}'='{getValue(v.Value)}'");
        }
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