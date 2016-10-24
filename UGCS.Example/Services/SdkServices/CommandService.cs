using Services.DTO;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol.Encoding;

namespace Services.SdkServices
{
    public class CommandService
    {
        private IConnect _connect;
        public enum Axis
        {
            //--- axes ---
            //vehicle
            VehiclePitch, //+1 - forward, -1 - backward
            VehicleRoll, //+1 - right, -1 - left
            VehicleElevator, //+1 - up, -1 - down
            VehicleYaw, //+1 - turn right, -1 - turn left
            //payload
            PayloadPitch, //+1 - up, -1 - down
            PayloadRoll, //+1 - right, -1 - left
            PayloadYaw, //+1 - turn right, -1 - turn left
            PayloadZoom, //+1 - zoom in, -1 - zoom out
            //payload2
            Payload2Pitch, //+1 - up, -1 - down
            Payload2Roll, //+1 - right, -1 - left
            Payload2Yaw, //+1 - turn right, -1 - turn left
            Payload2Zoom, //+1 - zoom in, -1 - zoom out

            //camera
            CameraHorizontalMoveRight, //+1 - right, -1 - left
            CameraHorizontalMoveForward,  //+1 - forward, -1 - backward
            CameraHorizontalRotationRight,  //+1 - right, -1 - left
            CameraHorizontalRotationForward,  //+1 - forward, -1 - backward
            CameraZoom, //+1 - zoom in, -1 - zoom out

            //--- buttons ---
            AutoMode,
            ManualMode,

            PayloadPowerOn,
            PayloadPowerOff,
            PayloadPowerToggle,
            PayloadTriggerShot,
            PayloadTriggerStartRec,
            PayloadTriggerStopRec,
            PayloadTriggerToggleRec,
            SelectVideoSource1,

            Payload2PowerOn,
            Payload2PowerOff,
            Payload2PowerToggle,
            Payload2TriggerShot,
            Payload2TriggerStartRec,
            Payload2TriggerStopRec,
            Payload2TriggerToggleRec,
            SelectVideoSource2,
        }

        public List<Tuple<Axis, String>> DirectVehicleControlAxes = new List<Tuple<Axis, String>>
		{
			new Tuple<Axis, String>(Axis.VehicleRoll, "roll"),
			new Tuple<Axis, String>(Axis.VehiclePitch, "pitch"),
			new Tuple<Axis, String>(Axis.VehicleYaw, "yaw"),
			new Tuple<Axis, String>(Axis.VehicleElevator, "throttle"),
		};

        public List<Tuple<Axis, String>> DirectCameraControlAxes = new List<Tuple<Axis, String>>
		{
			new Tuple<Axis, String>(Axis.PayloadRoll, "roll"),
			new Tuple<Axis, String>(Axis.PayloadPitch, "tilt"),
			new Tuple<Axis, String>(Axis.PayloadYaw, "yaw"),
			new Tuple<Axis, String>(Axis.PayloadZoom, "zoom"),
		};
        public CommandService(IConnect connect)
        {
            _connect = connect;
        }

        /// <summary>
        /// Lock vehicle vor current user connection
        /// </summary>
        /// <param name="id">vehicle id</param>
        /// <returns></returns>
        public AcquireLockResponse TryAcquireLock(int vehicleId)
        {
            AcquireLockRequest request = new AcquireLockRequest
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ObjectType = "Vehicle",
                ObjectId = vehicleId
            };

            var result = _connect.Executor.Submit<AcquireLockResponse>(request);
            result.Wait();
            return result.Value;
        }

        /// <summary>
        /// Release lock for vehicle
        /// </summary>
        /// <param name="vehicleId">Vehicle id</param>
        /// <param name="releaseIfSingleSession">Release for all user sessions</param>
        /// <returns></returns>
        public ReleaseLockResponse ReleaseLock(int vehicleId, bool releaseIfSingleSession = false)
        {            
            ReleaseLockRequest request = new ReleaseLockRequest
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ObjectType = "Vehicle",
                ObjectId = vehicleId,
                IfExclusive = releaseIfSingleSession
            };

            var result = _connect.Executor.Submit<ReleaseLockResponse>(request);
            result.Wait();
            return result.Value;
        }

        /// <summary>
        /// Send command to vehicle with lock request.
        /// </summary>
        /// <param name="vehicle">Vehicle object</param>
        /// <param name="commandDefinition">Command to send</param>
        /// <param name="addArgumentsFunc">Command parameters</param>
        /// <returns></returns>
        public SendCommandResponse SendLockCommand(Vehicle vehicle, CommandDefinition commandDefinition, Func<IEnumerable<CommandArgument>> addArgumentsFunc = null)
        {
            TryAcquireLock(vehicle.Id);
            var result = SendSingleCommand(vehicle, commandDefinition, addArgumentsFunc);
            return result;
        }

        /// <summary>
        /// Send single command to vehicle
        /// </summary>
        /// <param name="vehicle">Vehicle object</param>
        /// <param name="commandDefinition">Command to send</param>
        /// <param name="addArgumentsFunc">Command parameters</param>
        /// <returns></returns>
        public SendCommandResponse SendSingleCommand(Vehicle vehicle, CommandDefinition commandDefinition, Func<IEnumerable<CommandArgument>> addArgumentsFunc = null)
        {
            SendCommandRequest request = new SendCommandRequest
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                Command = new UGCS.Sdk.Protocol.Encoding.Command
                {
                    Code = commandDefinition.Code,
                    Subsystem = commandDefinition.Subsystem,
                }
            };
            request.Vehicles.Add(new Vehicle() { Id = vehicle.Id });
            if (addArgumentsFunc != null)
            {
                request.Command.Arguments.AddRange(addArgumentsFunc());
            }

            var responce = _connect.Executor.Submit<SendCommandResponse>(request);
            responce.Wait();

            if (responce.Value == null)
            {
                return null;
            }
            return responce.Value;
        }

        /// <summary>
        /// Send direct control payload command
        /// </summary>
        /// <param name="vehicle">Vehicle object</param>
        /// <param name="commandDefinition">Command to send</param>
        /// <param name="addArgumentsFunc">Command parameters</param>
        public void SendDirectControlCommand(Vehicle vehicle, CommandDefinition commandDefinition, Func<IEnumerable<CommandArgument>> addArgumentsFunc)
        {
            SendCommandRequest request = new SendCommandRequest
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                Command = new UGCS.Sdk.Protocol.Encoding.Command
                {
                    Code = commandDefinition.Code,
                    Subsystem = commandDefinition.Subsystem,
                    Silent = true,
                    ResultIndifferent = true
                }
            };
            request.Vehicles.Add(new Vehicle() { Id = vehicle.Id });
            if (addArgumentsFunc != null)
            {
                request.Command.Arguments.AddRange(addArgumentsFunc());
            }
            _connect.Executor.Submit<SendCommandResponse>(request);
        }
        
    }
}
