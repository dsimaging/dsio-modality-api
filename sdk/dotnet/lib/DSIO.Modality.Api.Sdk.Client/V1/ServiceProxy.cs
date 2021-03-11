using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using DSIO.Modality.Api.Sdk.Types.V1;

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

        #region Devices

        /// <summary>
        /// Retrieves all devices currently available to the service
        /// </summary>
        /// <returns>An IEnumerable collection of <see cref="DeviceInfo" /> objects</returns>
        public async Task<IEnumerable<DeviceInfo>> GetAllDevices()
        {
            var response = await Client.GetAsync("devices");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<DeviceInfo>>();
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
                return await response.Content.ReadAsAsync<DeviceInfo>();
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
                return await response.Content.ReadAsAsync<SensorInfo>();
            }

            return null;
        }

        #endregion

        #region Images

        /// <summary>
        /// Creates a session to be used for acquiring images.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="AcquisitionSessionInfo" /> describing
        /// the data to associate with the AcquisitionSession</param>
        /// <returns>A <see cref="AcquisitionSession" /> object</returns>
        public async Task<AcquisitionSession> CreateAcquisitionSession(AcquisitionSessionInfo sessionInfo)
        {
            var response = await Client.PostAsJsonAsync<AcquisitionSessionInfo>("acquisition", sessionInfo);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<AcquisitionSession>();
            }

            return null;
        }

        /// <summary>
        /// Retrieve an Acquisition Session
        /// </summary>
        /// <param name="sessionId">The Id of the session to retrieve</param>
        /// <returns>A <see cref="AcquisitionSession" /> object</returns>
        public async Task<AcquisitionSession> GetAcquisitionSession(string sessionId)
        {
            var response = await Client.GetAsync($"acquisition/{sessionId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<AcquisitionSession>();
            }

            return null;
        }

        /// <summary>
        /// Updates a session with new information. This method can be used to change
        /// The device used to acquire images in the session.
        /// </summary>
        /// <param name="sessionId">The Id of the session to update</param>
        /// <param name="sessionInfo">The <see cref="AcquisitionSessionInfo" /> describing
        /// the data to associate with the AcquisitionSession</param>
        /// <returns>A <see cref="AcquisitionSession" /> object</returns>
        public async Task<AcquisitionSession> UpdateAcquisitionSession(string sessionId, AcquisitionSessionInfo sessionInfo)
        {
            var response = await Client.PutAsJsonAsync<AcquisitionSessionInfo>($"acquisition/{sessionId}", sessionInfo);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<AcquisitionSession>();
            }

            return null;
        }

        /// <summary>
        /// Deletes an Acquisition Session. All images and data generated by the session
        /// will be deleted. Make sure that images have been downloaded and stored safely
        /// before deleting the session.
        /// </summary>
        /// <param name="sessionId">The Id of the session to delete</param>
        /// the data to associate with the AcquisitionSession</param>
        /// <returns>True if the request succeeded</returns>
        public async Task<bool> DeleteAcquisitionSession(string sessionId, AcquisitionSessionInfo sessionInfo)
        {
            var response = await Client.DeleteAsync($"acquisition/{sessionId}");
            return response.IsSuccessStatusCode;
        }

        #endregion
    }
}
