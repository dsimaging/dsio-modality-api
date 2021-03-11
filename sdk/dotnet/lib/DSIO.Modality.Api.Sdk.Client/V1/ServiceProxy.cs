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
    /// <summary>
    /// A class that wraps <see cref="HttpClient"> and provides async
    /// methods for sending API requests and receiving responses.
    /// </summary>
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

        /// <summary>
        /// Gets or sets the HttpClient used for API requests
        /// </summary>
        /// <value>HttpClient</value>
        public HttpClient Client { get; set; }

        /// <summary>
        /// Creates the proper Authorization header for use with API requests
        /// and adds the header to the <see cref="Client" />
        /// </summary>
        /// <param name="username">The API username</param>
        /// <param name="password">The API Key</param>
        public void SetBasicAuthenticationHeader(string username, string password)
        {
            // encode basic auth credentials as Base64
            var auth = $"{username}:{password}";
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));

            // Add default header to HttpClient
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);
        }

        /// <summary>
        /// Sends a request to the API Endpoint to check if the service is up an running
        /// </summary>
        /// <returns>true if the service is available, false otherwise</returns>
        public async Task<bool> IsServiceAvailable()
        {
            var response = await Client.GetAsync("");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Retrieves all devices currently available to the service
        /// </summary>
        /// <returns>An IEnumerable collection of <see cref="DeviceInfo" /> objects</returns>
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

        /// <summary>
        /// Retrieves information about a device
        /// </summary>
        /// <param name="id">The Id of the device to retrieve</param>
        /// <returns>A <see cref="DeviceInfo" /> object or null if the device is not found</returns>
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

        /// <summary>
        /// Retrieves information about a sensor connected to a device.
        /// </summary>
        /// <param name="id">The Id of the device</param>
        /// <returns>A <see cref="SensorInfo" /> object describing the connected sensor or null
        /// if no sensor is connected.</returns>
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
