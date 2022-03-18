using System.Diagnostics;
using UgCS.SDK.Examples.Common;
using UgCS.SDK.Examples.Takeoff;
using UGCS.Sdk.Protocol.Encoding;


// Connectung to UgCS Server (UCS).
using (UcsFacade ucs = UcsFacade.connectToUcs("localhost"))
{
    Console.WriteLine("Connected to ucs.");


    // Before take off a vehicle we have to get a reference to the corresponding vehicle.

    // List of vehicles connected to UCS at least once.
    var onlineVehicles = ucs.List<Vehicle>()
        // Keep only online vehicles supported 'takeoff_command'.
        .Where(v => ucs.IsOnline(v) && ucs.IsTakeoffSupported(v));

    Trace.Assert(
        onlineVehicles.Any(), 
        "To run this example at lease one drone that supports 'takeoff_command' must be connected to UgCS. " +
        "It may be a DJI vehicle with simulation enabled or Ardupilot SITL.");


    // Ask user to select a vehicle for take off.
    Vehicle selectedVehicle = ConsoleEx.Select(
        onlineVehicles,
        x => x.Id,
        x => x.Name);

    Trace.Assert(
        !ucs.IsArmed(selectedVehicle),
        $"The vehicle must not be armed.");


    // Before execute any command we have to get an exclusive control to the vehicle.
    ucs.AcquireLock(selectedVehicle);


    // Some drones (ardupilot, for example) must be armed before take off.
    // But not all drones support 'arm' command (for example, DJI drones don't).

    if (ucs.IsArmSupported(selectedVehicle))
    {
        // Before execute 'arm' make sure that the command is available
        // in the current vehicle state (the vehicle may be already armed, for example).
        if (!ucs.IsArmEnabled(selectedVehicle))
            throw new InvalidOperationException($"{selectedVehicle.Name}: 'arm' command isn't available in the current state.");
        ucs.Arm(selectedVehicle);
        Console.WriteLine($"{selectedVehicle.Name}: 'arm' executed successfully.");

        // ARM command executed successfully but before executing 'takeoff_command'
        // we must wait until the vehicle isarmed.
        ucs.WaitUntilArmed(selectedVehicle);
        Console.WriteLine($"{selectedVehicle.Name}: armed.");
    }

    if (!ucs.IsTakeoffEnabled(selectedVehicle))
        throw new InvalidOperationException($"{selectedVehicle.Name}: 'takeoff_command' command isn't available in the current state.");
    ucs.Takeoff(selectedVehicle);
    Console.WriteLine($"{selectedVehicle.Name}: 'takeoff_command' executed successfully.");
}
