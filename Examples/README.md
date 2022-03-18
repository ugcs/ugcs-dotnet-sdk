# UgCS .NET SDK - Examples

Each subdirectory of 'Examples' contains a simple console or graphic application which explains how to implement a certain thing with UgCS .NET SDK.


## Disclaimer
Those examples are for educational purposes only. We recommend running them with [SITL](https://ardupilot.org/dev/docs/sitl-simulator-software-in-the-loop.html) or UgCS emulators. We don't recommend using it with real drones.
However, if you decide to use it with a real drone, switch the drone to simulation mode.


## Running examples

### Prerequisites

Before run [examples](https://github.com/ugcs/ugcs-dotnet-sdk/tree/master/Examples) make sure that the following prerequisites are met.

- Git.
- [.NET Core SDK 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or newer.
- [UgCS 3.3](https://www.ugcs.com/download) or newer, activated with *UgCS PRO* or higher license.


### Running from terminal

1. Clone the repository.
2. Run UgCS.
3. Open a terminal. Cmd, for example.
4. Go into the example of the example you want run. For example, `'cd c:\ugcs-dotnet-sdk\Examples\Takeoff'`.
5. Call `'dotnet run'`.


### Running from Visual Studio

1. Open UgCS.SDK.Examples.sln.
2. In Solution Explorer select the project you want to run, and execute "Set as Startup Project" from the context menu.
3. Click Run (F5).
