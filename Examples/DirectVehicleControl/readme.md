# Direct vehicle control example
This example explains how to directly control a vehicle using `joystick` and `direct_vehicle_control` commands. It sends command to the Emu-101

## Prerequisites
[.NET Core SDK 2.1](https://dotnet.microsoft.com/download/dotnet/2.1) or newer.

## How to use
- Run UgCS.
- Select Emu-101 vehicle and execute ARM command (provide take-off altitude if required).
- Run the example code. The Emu-101 will gain altitude and rotate clockwise.


To run the example code execute `dotnet run` (from the folder where this documdent is located). 

---
**NOTE**

For DJI vehicles you must allow remote control (by clicking the button with a joystick icon) from the mobile app before send `direct_vehicle_control` command.

---