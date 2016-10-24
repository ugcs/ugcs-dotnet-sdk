using Services.DTO;
using Services.Interfaces;
using Services.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol.Encoding;

namespace Services.SdkServices
{
    public class VehicleService
    {
        ILogger logger = new Logger(typeof(VehicleService));
        private const String _invariantEntityName = "Vehicle";
        private IConnect _connect;

        public VehicleService(IConnect connect)
        {
            _connect = connect;
        }

        /// <summary>
        /// Example how to get vehicle by name.
        /// Function gets all vehicles from server and select specific vehicle
        /// </summary>
        /// <param name="name">Vehicle name</param>
        /// <returns></returns>
        public Vehicle GetVehicleByName(String name)
        {
            GetObjectListRequest request = new GetObjectListRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ObjectType = _invariantEntityName,
                RefreshDependencies = true
            };
            request.RefreshExcludes.Add("Avatar");
            request.RefreshExcludes.Add("PayloadProfile");
            request.RefreshExcludes.Add("Route");
            var task = _connect.Executor.Submit<GetObjectListResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
            }
            if (task.Value == null)
            {
                logger.LogWarningMessage("Could not retrieve list of vehicles");
                throw new Exception("Could not retrieve list of vehicles");
            }
            var list = task.Value;
            foreach (var vehicles in list.Objects)
            {
                if (String.Compare(name, vehicles.Vehicle.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return vehicles.Vehicle;
                }
            }
            throw new Exception("vehicle " + name + " not found");
        }

        /// <summary>
        /// Example how to get vehicle object by id
        /// </summary>
        /// <param name="vehicleId">vehicle id</param>
        /// <returns></returns>
        public Vehicle GetVehicleById(int vehicleId)
        {
            GetObjectRequest request = new GetObjectRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ObjectId = vehicleId,
                ObjectType = "Vehicle",
                RefreshDependencies = true,
            };
            request.RefreshExcludes.Add("Avatar");
            request.RefreshExcludes.Add("PayloadProfile");
            request.RefreshExcludes.Add("Route");
            var task = _connect.Executor.Submit<GetObjectResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
            }

            if (task.Value == null)
            {
                throw new Exception("Could not retrieve vehicle info: " + vehicleId);
            }
            return task.Value.Object.Vehicle;
        }

        /// <summary>
        /// Update vehicle fields on server
        /// </summary>
        /// <param name="vehicle">modified vehicle</param>
        /// <param name="fields">modiefied fields</param>
        /// <returns></returns>
        public bool SaveVehicleFields(Vehicle vehicle, IEnumerable<string> fields)
        {
            UpdateObjectFieldsRequest request = new UpdateObjectFieldsRequest
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ObjectType = "Vehicle",
                Object = new DomainObjectWrapper().Put(vehicle, "Vehicle"),
            };
            request.FieldNames.AddRange(fields);

            var task = _connect.Executor.Submit<UpdateObjectFieldsResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
            }
            if (task.Value == null)
            {
                logger.LogWarningMessage("Could not save vehicle info: " + vehicle.Name);
                throw new Exception("Could not save vehicle info: " + vehicle.Name);
            }
            return true;
        }

        /// <summary>
        /// Update vehicle on server
        /// </summary>
        /// <param name="vehicle">Modified vehicle</param>
        /// <returns></returns>
        public bool SaveVehicle(Vehicle vehicle)
        {
            CreateOrUpdateObjectRequest request = new CreateOrUpdateObjectRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                Object = new DomainObjectWrapper().Put(vehicle, "Vehicle"),
                WithComposites = true,
                ObjectType = "Vehicle",
                AcquireLock = false
            };
            var task = _connect.Executor.Submit<CreateOrUpdateObjectResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
            }

            if (task.Value == null)
            {
                logger.LogWarningMessage("Could not save vehicle info: " + vehicle.Name);
                throw new Exception("Could not save vehicle info: " + vehicle.Name);
            }
            return true;
        }

        /// <summary>
        /// Selete vehicle from server
        /// </summary>
        /// <param name="vehicle">Vehicle to delete</param>
        /// <returns></returns>
        public bool DeleteVehicle(Vehicle vehicle)
        {
            DeleteObjectRequest request = new DeleteObjectRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ObjectId = vehicle.Id,
                ObjectType = "Vehicle"
            };
            var task = _connect.Executor.Submit<DeleteObjectResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
            }

            if (task.Value == null)
            {
                logger.LogWarningMessage("Could not delete vehicle: " + vehicle.Name);
                throw new Exception("Could not delete vehicle: " + vehicle.Name);
            }
            return true;
        }
    }
}
