using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using DSIO.Modality.Api.Sdk.Types.V1;
using Newtonsoft.Json;

namespace DSIO.Modality.Api.Sdk.Client.V1
{
    public class ServiceProxy
    {
        public ServiceProxy()
        {
            Client = new HttpClient()
            {
                // Initialize BaseAddress to known endpoint for Modality Api V1 in IO Sensor Software
                BaseAddress = new Uri(@"https://localhost:43809/api/dsio/modality/v1/")
            };
        }

        public HttpClient Client { get; set; }

        public void SetBasicAuthenticationHeader(string username, string password)
        {
            // encode basic auth credentials as Base64
            var auth = $"{username}:{password}";
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));

            // Add default header to HttpClient
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);
        }

        public async Task<bool> IsServiceAvailable()
        {
            var response = await Client.GetAsync("");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<DeviceInfo>> GetAllDevices()
        {
            var response = await Client.GetAsync("devices");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<DeviceInfo>>(json);
            }

            return null;
        }
        public async Task<DeviceInfo> GetDevice(string id)
        {
            var response = await Client.GetAsync($"devices/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DeviceInfo>(json);
            }

            return null;
        }

        public async Task<SensorInfo> GetSensor(string id)
        {
            var response = await Client.GetAsync($"devices/{id}/sensor");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SensorInfo>(json);
            }

            return null;
        }

    }
}
