using Com.Ugcs.Ucs.Proto;
using UgCS.SDK.Examples.Common;

using (var ucs = UcsFacade.connectToUcs("localhost"))
{
    var route = ucs.List<Route>().FirstOrDefault() ?? 
        throw new ApplicationException("At least one route must be created in UgCS to execute this example.");

    // This object is just a stub.
    // It does not contain any field values except the 'id' and 'tag'.
    var missionStub = route.Mission;

    missionStub.GetType().GetProperties()
        .Where(p => p.Name != null && p.Name.Contains("Specified"))
        .Select(p => new { Name = p.Name, Value = p.GetValue(missionStub) })
        .OrderByDescending(it => it.Value)
        .ToList()
        .ForEach(p => Console.WriteLine($"{p.Name}: {p.Value}"));
}