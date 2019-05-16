using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TrunkAPIHandler
{
    public class Trunk
    {
        public string Gateway { get; set; }
        public string State { get; set; }

        [JsonProperty(PropertyName = "Ping-Status")]
        public string PingStatus { get; set; }

        [JsonProperty(PropertyName = "FreeSWITCH-Hostname")]
        public string ConnectedTo { get; set; }

        [JsonProperty(PropertyName = "FreeSWITCH-IPv4")]
        public string MediaServerIp { get; set; }
        
        [JsonProperty(PropertyName = "Event-Date-Local")]
        public string LastUpdated { get; set; }
        
    }

    public class TrunkHandler
    {
        public List<Trunk> GetTrunks()
        {
            string monitorRestApiUrl = ConfigurationManager.AppSettings["MonitorRestAPIUrl"] + "/TrunkMonitoring/Trunks";
            string token = ConfigurationManager.AppSettings["Token"];
            string companyInfo = ConfigurationManager.AppSettings["CompanyInfo"];
            List<Trunk> trunkList = new List<Trunk>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(monitorRestApiUrl);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("CompanyInfo", companyInfo);

            // List data response.
            HttpResponseMessage response = client.GetAsync(monitorRestApiUrl).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsStringAsync().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll

                var trunkResult = JObject.Parse(dataObjects);

                trunkList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Trunk>>(trunkResult.GetValue("Result").ToString());
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            //Make any other calls using HttpClient here.

            //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();

            return trunkList;
        }
    }
}
