using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

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
        /// Elevation source that will be created and used in this example
        /// </summary>
        private static string ELEVATION_SOURCE = "new_source";

        /// <summary>
        /// Elevation source that will be created and used in this example
        /// </summary>
        private static string SAMPLE_ELEVATION_FILE = "sample.zip";

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

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                // Now this is unexpected...
                System.Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// POST request to create a new source
        /// It has no body
        /// </summary>
        private static void createElevationSource()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(
                string.Format("{0}/{1}/elevation?name=MyFriendlyName&description=NondescriptSource", 
                GEOSERVER_ADDRESS, ELEVATION_SOURCE));
            httpWebRequest.Method = "POST";

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                // See what wend south
                System.Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// POST request to create a new source
        /// It has no body
        /// </summary>
        private static int? uploadElevation()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("{0}/{1}/elevation", GEOSERVER_ADDRESS, ELEVATION_SOURCE));
            httpWebRequest.ContentType = "application/zip";
            httpWebRequest.Method = "PUT";
            httpWebRequest.AllowWriteStreamBuffering = false;

            try
            {
                using (FileStream fileStream = new FileStream(SAMPLE_ELEVATION_FILE, FileMode.Open, FileAccess.Read))
                {
                    httpWebRequest.ContentLength = fileStream.Length;

                    using (var requestStream = httpWebRequest.GetRequestStream())
                    {
                        byte[] buffer = new byte[4096];
                        while (true)
                        {
                            int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                            if (bytesRead > 0)
                            {
                                requestStream.Write(buffer, 0, bytesRead);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    // Get the response string
                    var result = streamReader.ReadToEnd();
                    // Get the part with ID
                    var idString = Regex.Match(result, @"""id\"":\d+").Value;
                    // Now get the operation ID number from it
                    int id = int.Parse(Regex.Match(idString, @"\d+").Value);
                    return id;
                }
            }
            catch (Exception e)
            {
                // Do something nice here
                System.Console.WriteLine(e.Message);
            }
            return null;
        }

        /// <summary>
        /// GET request to get operation status
        /// It has no body
        /// Can be called regularly to query what's going on with a specific op
        /// </summary>
        private static void queryOp(int operationId)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(
                string.Format("{0}?statusOf={1}",
                GEOSERVER_ADDRESS, operationId));
            httpWebRequest.Method = "GET";

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    // This contains all the status information
                    var result = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                // Unusual for this to go haywire, but ok...
                System.Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            addPlacemark();
            createElevationSource();
            int? uploadOpId = uploadElevation();
            if(uploadOpId.HasValue)
            {
                queryOp(uploadOpId.Value);
            }
        }
    }
}
