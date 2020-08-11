using System.IO;
using System.Net;

namespace Geoserver
{
    class Program
    {
        /// <summary>
        /// Geoserver address
        /// </summary>
        private static string GEOSERVER_ADDRESS = "http://127.0.0.1:8079";

        /// <summary>
        /// Placemark layer where to put placemarks
        /// </summary>
        private static string PLACEMARK_LAYER = "default";

        /// <summary>
        /// PUT json with placemark information to specified layer.
        /// </summary>
        private static void addPlacemark()
        {
            // symbolId - placemark ID defined in geoserver. 
            // imageId - custom placemark image that was uploaded to geoserver before.
            var geoServerRequest = @"[
    {""altitude"": 10, 
    ""altitudeMode"": ""aboveTerrain"", 
    ""description"": ""Sample description"", 
    ""latitude"": ""46.7754254"", 
    ""longitude"": ""8.3451136"", 
    ""symbolId"": ""101"", 
    ""imageId"": """"}
]";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("{0}/{1}/placemark", GEOSERVER_ADDRESS, PLACEMARK_LAYER));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(geoServerRequest);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }

        static void Main(string[] args)
        {
            addPlacemark();
        }
    }
}
