using UGCS.Sdk.Protocol.Encoding;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Services.SdkServices.ServerModels
{
	public class AlgorithmTypeModel : ServerSingleModelProvider<AlgorithmTypeModel, GetMappingRequest, GetMappingResponse>
    {
        private VehicleProfile _profile;
		protected override GetMappingRequest Request
        {
			get
			{
				return new GetMappingRequest
				{
					ClientId = _connection.AuthorizeHciResponse.ClientId,
					GetAlgorithms = true
				};
			}
        }


        public void Reset()
        {
            Init();
        }

        public TraverseAlgorithm CurrentAlgorithm(int algorithmId)
        {
			if (_profile == null || _response == null || _response.AlgorithmsMapping == null || _response.AlgorithmsMapping.Algorithms == null)
                return null;
            AlgorithmsDto algorithmsDto = _response
				.AlgorithmsMapping
				.Algorithms
				.FirstOrDefault(algorithm => 
					algorithm.PlatformAndVehicleType.Platform.Id == _profile.Platform.Id && 
					algorithm.PlatformAndVehicleType.VehicleType == _profile.VehicleType);
			if (algorithmsDto == null)
				return null;
			return algorithmsDto
				.AlgorithmAndActionCodes
				.Where(id => id.Algorithm.Id == algorithmId)
				.Select(x => x.Algorithm)
				.FirstOrDefault();
        }

		public TraverseAlgorithm GetAlgoritmByClassName(String algorithmClassName)
		{
			foreach (AlgorithmsDto algorithmsDto in _response.AlgorithmsMapping.Algorithms)
			{
				TraverseAlgorithm algorithm = algorithmsDto.AlgorithmAndActionCodes
					.Where(id => id.Algorithm.GetClassName() == algorithmClassName)
					.Select(x => x.Algorithm)
					.FirstOrDefault<TraverseAlgorithm>();
				if (algorithm != null)
					return algorithm;
			}
			return null;
		}

        public List<TraverseAlgorithm> GetAlgoritmsForPlatform(VehicleProfile vehicleProfile)
        {
            _profile = vehicleProfile;
			if (_response == null || _response.AlgorithmsMapping == null || _response.AlgorithmsMapping.Algorithms == null)
            {
                return new List<TraverseAlgorithm>();
            }

            var platform = _response
				.AlgorithmsMapping
				.Algorithms
				.FirstOrDefault(algotithm => 
					algotithm.PlatformAndVehicleType.Platform.Id == _profile.Platform.Id && 
					algotithm.PlatformAndVehicleType.VehicleType == _profile.VehicleType);
			return platform != null
				? platform.AlgorithmAndActionCodes.Select(x => x.Algorithm).OrderBy(o => o.Order).ToList()
				: new List<TraverseAlgorithm>();
        }
    }
}