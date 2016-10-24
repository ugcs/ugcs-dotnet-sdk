using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGCS.Sdk.Protocol.Encoding;

namespace Services.SdkServices
{
    public class MappingRequestService
    {
        protected GetMappingResponse _responseAlghoritms;

        private IConnect _connect;
        public MappingRequestService(IConnect connect)
        {
            _connect = connect;
            _requestAlgorithms();
        }

        /// <summary>
        /// Request all availabe algorithms from UCS server
        /// </summary>
        private void _requestAlgorithms()
        {
            var request = new GetMappingRequest()
            {
                ClientId = _connect.AuthorizeHciResponse.ClientId,
                GetAlgorithms = true
            };
            var response = _connect.Executor.Submit<GetMappingResponse>(request);
            response.Wait();
            _responseAlghoritms = response.Value;
        }


        /// <summary>
        /// Returns avaliable waypoint algorithm object by class name
        /// </summary>
        /// <param name="algorithmClassName">algorithms lass name</param>
        /// <returns></returns>
        public TraverseAlgorithm GetAlgoritmByClassName(String algorithmClassName)
        {
            foreach (AlgorithmsDto algorithmsDto in _responseAlghoritms.AlgorithmsMapping.Algorithms)
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

    }
}
