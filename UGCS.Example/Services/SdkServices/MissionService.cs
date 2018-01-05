using Services.DTO;
using Services.Interfaces;
using Services.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Sdk.Protocol;
using Services.Helpers;

namespace Services.SdkServices
{
    public class MissionService
    {
        private ILogger logger = new Logger(typeof(MissionService));
        private const String _invariantEntityName = "Mission";
        private IConnect _connect;
        private Mission _mission;

        public MissionService(IConnect connect, Mission mission)
        {
            _connect = connect;
        }        

        /// <summary>
        /// Creates new mission
        /// </summary>
        /// <param name="name">Mission name</param>
        /// <returns></returns>
        public Mission CreateNewMission(String name = "TestMission")
        {
            var missionName = string.Format("{0} {1}", name, DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());

            Mission mission = new Mission
            {
                CreationTime = ServiceHelpers.CreationTime(),
                Name = missionName,
                Owner = _connect.LoginResponce.User
            };
            return _saveUpdatedMission(mission);
        }        

        /// <summary>
        /// Example how to list missions and get mission by name
        /// </summary>
        /// <param name="name">Mission name</param>
        /// <returns>Selected mission by  name</returns>
        public Mission GetMissionByName(String name)
        {
            _mission = null;
            GetObjectListRequest request = new GetObjectListRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ObjectType = _invariantEntityName,
                RefreshDependencies = false
            };
            var task = _connect.Executor.Submit<GetObjectListResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
            }
            if (task.Value == null)
            {
                logger.LogWarningMessage("Could not retrieve list of missions");
                throw new Exception("Could not retrieve list of missions");
            }
            var list = task.Value;
            foreach (var mission in list.Objects)
            {
                if (mission.Mission.Name == name)
                {
                    _mission = mission.Mission;
                    break;
                }
            }
            if (_mission == null)
            {
                logger.LogWarningMessage("Mission " + name + " not found");
                return null;
            }

            return GetMissionPreferences(_mission);
        }

        /// <summary>
        /// Example how to get mission preferences with dependencies
        /// </summary>
        /// <param name="mission">Currect mission</param>
        /// <returns>Mission with prefrences</returns>
        public Mission GetMissionPreferences(Mission mission)
        {
            GetObjectRequest request = new GetObjectRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ObjectId = mission.Id,
                ObjectType = _invariantEntityName,
                RefreshDependencies = false
            };
            var task = _connect.Executor.Submit<GetObjectResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
            }
            if (task.Value == null)
            {
                logger.LogWarningMessage("Could not retrieve mission info: " + mission.Name);
                throw new Exception("Could not retrieve mission info: " + mission.Name);
            }

            return task.Value.Object.Mission;
        }

        /// <summary>
        /// Example how to update mission
        /// </summary>
        /// <param name="missionId">Mission ID</param>
        /// <returns>success or falce mission refresh</returns>
        public bool RefreshMission(int missionId)
        {
            GetObjectRequest request = new GetObjectRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                ObjectId = missionId,
                ObjectType = "Mission",
                RefreshDependencies = true,
            };
            request.RefreshExcludes.Add("PayloadProfile");
            request.RefreshExcludes.Add("Platform");
            request.RefreshExcludes.Add("User");
            request.RefreshExcludes.Add("Vehicle");
            request.RefreshExcludes.Add("VehicleProfile");
            var task = _connect.Executor.Submit<GetObjectResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
            }
            if (task.Value == null)
            {
                logger.LogWarningMessage("Could not retrieve route info: " + missionId);
                throw new Exception("Could not retrieve route info: " + missionId);
            }

            CreateOrUpdateObjectRequest requestSave = new CreateOrUpdateObjectRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                Object = new DomainObjectWrapper().Put(task.Value.Object.Mission, "Mission"),
                ObjectType = "Mission",
                AcquireLock = false
            };
            var taskSave = _connect.Executor.Submit<CreateOrUpdateObjectResponse>(requestSave);
            taskSave.Wait();
            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
            }
            if (taskSave.Value == null)
            {
                logger.LogWarningMessage("Could not save route info: " + task.Value.Object.Mission);
                throw new Exception("Could not save route info: " + task.Value.Object.Mission);
            }

            return true;
        }

        /// <summary>
        /// Example how to create or update mission on server
        /// </summary>
        /// <param name="mission">unsaved mission</param>
        /// <returns>new saved mission</returns>
        private Mission _saveUpdatedMission(Mission mission)
        {
            CreateOrUpdateObjectRequest request = new CreateOrUpdateObjectRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                Object = new DomainObjectWrapper().Put(mission, "Mission"),
                WithComposites = true,
                ObjectType = "Mission",
                AcquireLock = false
            };
            var task = _connect.Executor.Submit<CreateOrUpdateObjectResponse>(request);
            task.Wait();

            if (task.Exception != null)
            {
                logger.LogException(task.Exception);
                throw new Exception("Save error: " + task.Exception.Message);
            }

            if (task.Value == null)
            {
                logger.LogWarningMessage("Could not save route info: " + mission.Name);
                throw new Exception("Could not save route info: " + mission.Name);
            }

            return task.Value.Object.Mission;
        }    
    }
}
