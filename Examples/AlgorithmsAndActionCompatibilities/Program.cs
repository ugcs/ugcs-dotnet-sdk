using Com.Ugcs.Ucs.Proto;
using System.Linq;
using UgCS.SDK.Examples.Common;

using (var ucs = UcsFacade.connectToUcs("localhost"))
{
    Console.WriteLine(
        "Algoritm and actions compatibility list below contains a platform on the top level, " +
        "compatible algorithms at the next level, and compatible actions for the corresponding " +
        "algorighm on the last level.");
    Console.WriteLine();


    var algMapping = ucs.Execute<GetMappingRequest, GetMappingResponse>(new GetMappingRequest
    {
        ClientId = ucs.ClientId,
        GetAlgorithms = true
    })
        .AlgorithmsMapping
        .Algorithms
        .Where(x => x.PlatformAndVehicleType.VehicleType == VehicleType.VtMulticopter);


    foreach (var x in algMapping.OrderBy(x => x.PlatformAndVehicleType.Platform.Name))
    {
        Console.WriteLine(x.PlatformAndVehicleType.Platform.Name);
        foreach (var y in x.AlgorithmAndActionCodes)
        {
            Console.WriteLine("  " + y.Algorithm.Name);
            foreach (var action in y.ActionCodes)
            {
                Console.WriteLine("    " + action);
            }
        }
    }

}