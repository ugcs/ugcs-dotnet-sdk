using Com.Ugcs.Ucs.Proto;
using System.Diagnostics;
using System.Linq;
using UgCS.SDK.Examples.Common;

using (var ucs = UcsFacade.connectToUcs("localhost"))
{
    var onlineVehicles = ucs.List<Vehicle>()
            .Where(v => ucs.IsOnline(v));

    // Ask user to select a vehicle for take off.
    Vehicle selectedVehicle = ConsoleEx.Select(
        onlineVehicles,
        x => x.Id,
        x => x.Name);

    while (true)
    {
        CommandDefinition? commandDef = null;
        while (commandDef == null)
        {
            Console.Write("Input command code: ");
            String? commandCode = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(commandCode))
                continue;

            commandDef = ucs.GetCommands(selectedVehicle)
                    .SingleOrDefault(x => x.Code == commandCode);
            if (commandDef == null)
            {
                Console.WriteLine(
                    "There is no command with code '{0}' registered by '{1}'.",
                    commandCode,
                    selectedVehicle.Name);
            }
        }

        try
        {
            var req = new SendCommandRequest
            {
                Command = new Command
                {
                    Code = commandDef.Code,
                    Subsystem = commandDef.Subsystem,
                    SubsystemId = commandDef.SubsystemId
                }
            };


            req.Vehicles.Add(selectedVehicle);
            var response = ucs.Execute<SendCommandRequest, SendCommandResponse>(req);
            var commandResult = response.CommandResults.Single();
            Console.WriteLine("Execution status: " + commandResult.CommandStatus);
            if (commandResult.HasErrorMessage)
                Console.WriteLine(commandResult.ErrorMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Command execution failed:");
            Console.WriteLine(ex);
        }
    }
}