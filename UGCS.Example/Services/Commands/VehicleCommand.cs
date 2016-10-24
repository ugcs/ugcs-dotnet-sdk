using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol.Encoding;

namespace Services.Commands
{
    public class VehicleCommand
    {
        private class Commands //note: these command names are used in TimeLine too
        {
            // Flight Controller Commands
            public static readonly CommandDefinition Arm = new CommandDefinition { Code = "arm", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition Disarm = new CommandDefinition { Code = "disarm", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition Auto = new CommandDefinition { Code = "auto", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition Manual = new CommandDefinition { Code = "manual", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition Guided = new CommandDefinition { Code = "guided", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition Joystick = new CommandDefinition { Code = "joystick", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition Takeoff = new CommandDefinition { Code = "takeoff_command", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition Land = new CommandDefinition { Code = "land_command", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition EmergencyLand = new CommandDefinition { Code = "emergency_land", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition ReturnHome = new CommandDefinition { Code = "return_to_home", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition Pause = new CommandDefinition { Code = "mission_pause", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition Resume = new CommandDefinition { Code = "mission_resume", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition Waypoint = new CommandDefinition { Code = "waypoint", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition DirectVehicleControl = new CommandDefinition { Code = "direct_vehicle_control", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };
            public static readonly CommandDefinition SelectAsVideoSource = new CommandDefinition { Code = "select_as_video_source", Subsystem = Subsystem.S_FLIGHT_CONTROLLER };

            // Camera Commands
            public static readonly CommandDefinition CameraTrigger = new CommandDefinition { Code = "camera_trigger_command", Subsystem = Subsystem.S_CAMERA };
            public static readonly CommandDefinition CameraPower = new CommandDefinition { Code = "camera_power", Subsystem = Subsystem.S_CAMERA };
            public static readonly CommandDefinition DirectPayloadControl = new CommandDefinition { Code = "direct_payload_control", Subsystem = Subsystem.S_GIMBAL };

            // ADS-B Transponder Commands
            public static readonly CommandDefinition AdsbOperating = new CommandDefinition { Code = "adsb_operating", Subsystem = Subsystem.S_ADSB_TRANSPONDER };
            public static readonly CommandDefinition AdsbInstall = new CommandDefinition { Code = "adsb_install", Subsystem = Subsystem.S_ADSB_TRANSPONDER };
            public static readonly CommandDefinition AdsbPreflight = new CommandDefinition { Code = "adsb_preflight", Subsystem = Subsystem.S_ADSB_TRANSPONDER };
        }

        public enum CommandName
        {
            Arm = 0,
            AutoMode = 1,
            ManualMode = 2,
            Waypoint = 3,
            Takeoff = 4,
            Land = 5,
            ReturnHome = 6,
            Disarm = 7,
            Hold = 8,
            Continue = 9,
            EmergencyLand = 10,
            CameraTrigger = 11,
            AdsbOperating = 12,
            AdsbTransponderInstall = 13,
            AdsbTransponderPreflight = 14,
            GuidedMode = 15,
            DirectVehicleControl = 16,
            DirectPayloadControl = 17,
            CameraPower = 18,
            SelectVideoSource = 19,
            JoystickMode = 20
        }

        private Dictionary<CommandName, CommandDefinition> _commandDefinitions = new Dictionary<CommandName, CommandDefinition>
        {
            { CommandName.Arm, Commands.Arm },
            { CommandName.AutoMode, Commands.Auto },
            { CommandName.ManualMode, Commands.Manual },
            { CommandName.Waypoint, Commands.Waypoint },
            { CommandName.Takeoff, Commands.Takeoff },
            { CommandName.Land, Commands.Land },
            { CommandName.ReturnHome, Commands.ReturnHome },
            { CommandName.Disarm, Commands.Disarm },
            { CommandName.Hold, Commands.Pause },
            { CommandName.Continue, Commands.Resume },
            { CommandName.EmergencyLand, Commands.EmergencyLand },
            { CommandName.CameraTrigger, Commands.CameraTrigger },
            { CommandName.AdsbOperating, Commands.AdsbOperating },
            { CommandName.AdsbTransponderInstall, Commands.AdsbInstall },
            { CommandName.AdsbTransponderPreflight, Commands.AdsbPreflight },
            { CommandName.GuidedMode, Commands.Guided },
            { CommandName.DirectVehicleControl, Commands.DirectVehicleControl },
            { CommandName.DirectPayloadControl, Commands.DirectPayloadControl },
            { CommandName.CameraPower, Commands.CameraPower },
            { CommandName.SelectVideoSource, Commands.SelectAsVideoSource },
            { CommandName.JoystickMode, Commands.Joystick },
        };

        private Dictionary<String, CommandName> _commandsByCode = new Dictionary<String, CommandName>
        {
            { Commands.Arm.Code, CommandName.Arm },
            { Commands.Auto.Code, CommandName.AutoMode },
            { Commands.Manual.Code, CommandName.ManualMode },
            { Commands.Waypoint.Code, CommandName.Waypoint },
            { Commands.Takeoff.Code, CommandName.Takeoff },
            { Commands.Land.Code, CommandName.Land },
            { Commands.ReturnHome.Code, CommandName.ReturnHome },
            { Commands.Disarm.Code, CommandName.Disarm },
            { Commands.Pause.Code, CommandName.Hold },
            { Commands.Resume.Code, CommandName.Continue },
            { Commands.EmergencyLand.Code, CommandName.EmergencyLand },
            { Commands.CameraTrigger.Code, CommandName.CameraTrigger },
            { Commands.AdsbOperating.Code, CommandName.AdsbOperating },
            { Commands.AdsbInstall.Code, CommandName.AdsbTransponderInstall },
            { Commands.AdsbPreflight.Code, CommandName.AdsbTransponderPreflight },
            { Commands.Guided.Code, CommandName.GuidedMode },
            { Commands.DirectVehicleControl.Code, CommandName.DirectVehicleControl },
            { Commands.DirectPayloadControl.Code, CommandName.DirectPayloadControl },
            { Commands.CameraPower.Code, CommandName.CameraPower },
            { Commands.SelectAsVideoSource.Code, CommandName.SelectVideoSource },
            { Commands.Joystick.Code, CommandName.JoystickMode },
        };

        public CommandDefinition GetCommand(CommandName commandName)
        {
            if (!_commandDefinitions.ContainsKey(commandName)) {
                
                throw new ArgumentException("Command code '" + commandName.ToString() + "' not found.");
            }
            return _commandDefinitions[commandName];
        }
    }
}
