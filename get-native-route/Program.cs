

// Connectung to UgCS Server (UCS).
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;
using UgCS.SDK.Examples.Common;
using UGCS.Sdk.Protocol.Encoding;
using CommandArgument = Microsoft.Extensions.CommandLineUtils.CommandArgument;


static class Program
{
    public static void Main(string[] args)
    {
        var app = new CommandLineApplication
        {
            Name = "get-native-route.exe",
            Description = "Returns native route resolver by the VSM for the input UgCS route."
        };
        app.VersionOption("-v|--version", "1.0");
        CommandArgument inputPath = app.Argument("<input-path>", "The full path to the .json file with the route exported from UgCS.");
        CommandOption host = app.Option("-h|--host", "The host name or ip address of the UCS. By default 'localhost'.", CommandOptionType.SingleValue);

        app.OnExecute(() =>
        {
            if (inputPath.Value == null)
            {
                app.ShowHelp();
                return 0;
            }

            using (UcsFacade ucs = UcsFacade.connectToUcs(host.HasValue() ? host.Value() : "localhost"))
            {
                Route route = ucs.LoadRoute(inputPath.Value);
                ProcessedRoute processedRoute = ucs.Process(route);
                byte[] nativeRoute = ucs.GetNativeRoute(processedRoute, route.VehicleProfile);

                using (var nativeRouteStream = new MemoryStream(nativeRoute))
                {
                    using (var reader = new StreamReader(nativeRouteStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            Console.WriteLine(reader.ReadLine());
                        }
                    }
                }
            }

            return 0;
        });

        try
        {
            app.Execute(args);
        }
        catch (CommandParsingException ex)
        {
            Console.WriteLine(ex.Message);
            app.ShowHelp();
        }


    }
}