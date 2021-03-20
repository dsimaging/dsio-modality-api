using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
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
            // This method is intended to simply check if the service is running.
            // When not running, an HTTP Request will generate a Socket exception
            // with the error status ConnectionRefused. Handle this exception and
            // return false, but rethrow all other exceptions.
            try
            {
                var response = await Client.GetAsync("");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Check for expected exception when service not running (connection refused)
                // iterate through all InnerExceptions looking for SocketException
                var innerException = ex;
                while (innerException != null)
                {
                    // Check for SocketException with ConnectionRefused error
                    if (innerException is SocketException socketException &&
                        socketException.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        // Expected error code when service is not running, just return false
                        return false;
                    }
                    innerException = innerException.InnerException;
                }

                // rethrow exception if we land here
                throw;
            }
        }

        #region Devices

        /// <summary>
        /// Retrieves all devices currently available to the service
        /// </summary>
        /// <returns>An IEnumerable collection of <see cref="DeviceInfo" /> objects</returns>
        public async Task<IEnumerable<DeviceInfo>> GetAllDevices()
        {
            var response = await Client.GetAsync("devices");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<IEnumerable<DeviceInfo>>();
        }

        /// <summary>
        /// Creates a subscription that listens for Device Events.
        /// </summary>
        /// <param name="onDeviceEvent">A callback method that will be called for each event received</param>
        /// <param name="heartbeat">Optional number specifying desired heartbeat frequency in ms. Default
        ///  value is 20000.
        /// <returns>A subscription <see cref="ISubscription" /></returns>
        public async Task<ISubscription> SubscribeToDeviceEvents(Action<DeviceEventData> onDeviceEvent, int heartbeat = 20000)
        {
            // Use a dedicated instance of HttpClient to keep the connection open
            var client = new HttpClient
            {
                BaseAddress = Client.BaseAddress
            };
            client.DefaultRequestHeaders.Authorization = Client.DefaultRequestHeaders.Authorization;

            var stream = await client.GetStreamAsync($"devices/subscribe?heartbeat={heartbeat}");
            return new DeviceEventSubscription(stream, onDeviceEvent);
        }

        /// <summary>
        /// Retrieves information about a device
        /// </summary>
        /// <param name="id">The Id of the device to retrieve</param>
        /// <returns>A <see cref="DeviceInfo" /> object or null if the device is not found</returns>
        public async Task<DeviceInfo> GetDevice(string id)
        {
            var response = await Client.GetAsync($"devices/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<DeviceInfo>();
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
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<SensorInfo>();
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
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AcquisitionSession>();
        }

        /// <summary>
        /// Retrieve an Acquisition Session
        /// </summary>
        /// <param name="sessionId">The Id of the session to retrieve</param>
        /// <returns>A <see cref="AcquisitionSession" /> object</returns>
        public async Task<AcquisitionSession> GetAcquisitionSession(string sessionId)
        {
            var response = await Client.GetAsync($"acquisition/{sessionId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AcquisitionSession>();
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
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AcquisitionSession>();
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

        /// <summary>
        /// Retrieves the current status of the Acquisition session. 
        /// </summary>
        /// <param name="sessionId">The Id of the session</param>
        /// <returns>A <see cref="AcquisitionStatus" /> object</returns>
        public async Task<AcquisitionStatus> GetAcquisitionStatus(string sessionId)
        {
            var response = await Client.GetAsync($"acquisition/{sessionId}/status");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AcquisitionStatus>();
        }

        /// <summary>
        /// Creates a subscription that listens for Acquisition Status.
        /// </summary>
        /// <param name="sessionId">The Id of the session</param>
        /// <param name="onStatus">A callback method that will be called for each event received</param>
        /// <param name="heartbeat">Optional number specifying desired heartbeat frequency in ms. Default
        ///  value is 1000.
        /// <returns>A subscription <see cref="ISubscription" /></returns>
        public async Task<ISubscription> SubscribeToAcquisitionStatus(string sessionId, Action<AcquisitionStatus> onStatus, int heartbeat = 1000)
        {
            // Use a dedicated instance of HttpClient to keep the connection open
            var client = new HttpClient
            {
                BaseAddress = Client.BaseAddress
            };
            client.DefaultRequestHeaders.Authorization = Client.DefaultRequestHeaders.Authorization;

            var stream = await client.GetStreamAsync($"acquisition/{sessionId}/status/subscribe?heartbeat={heartbeat}");
            return new AcquisitionStatusSubscription(stream, onStatus);
        }

        /// <summary>
        /// Retrieves <see cref="AcquisitionInfo" /> associated with the next exposure. 
        /// </summary>
        /// <param name="sessionId">The Id of the session</param>
        /// <returns>A <see cref="AcquisitionInfo" /> object</returns>
        public async Task<AcquisitionInfo> GetAcquisitionInfo(string sessionId)
        {
            var response = await Client.GetAsync($"acquisition/{sessionId}/info");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AcquisitionInfo>();
        }

        /// <summary>
        /// Updates the <see cref="AcquisitionInfo" /> associated with the next exposure. 
        /// </summary>
        /// <param name="sessionId">The Id of the session</param>
        /// <param name="acquisitionInfo">The <see cref="AcquisitionInfo" /> to update.
        /// <returns>The updated <see cref="AcquisitionInfo" /> object</returns>
        public async Task<AcquisitionInfo> UpdateAcquisitionInfo(string sessionId, AcquisitionInfo acquisitionInfo)
        {
            var response = await Client.PutAsJsonAsync<AcquisitionInfo>($"acquisition/{sessionId}/info", acquisitionInfo);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AcquisitionInfo>();
        }

        /// <summary>
        /// Retrieves all images for a session.
        /// </summary>
        /// <param name="sessionId">The Id of the sesion</param>
        /// <returns>An enumerable collection of <see cref="ImageInfo" /> objects</returns>
        public async Task<IEnumerable<ImageInfo>> GetAllImages(string sessionId)
        {
            var response = await Client.GetAsync($"acquisition/{sessionId}/images");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<IEnumerable<ImageInfo>>();
        }

        /// <summary>
        /// Retrieves an image from a session.
        /// </summary>
        /// <param name="sessionId">The Id of the session</param>
        /// <param name="imageId">The Id of the image to retrieve</param>
        /// <returns>An <see cref="ImageInfo" /> object</returns>
        public async Task<ImageInfo> GetImage(string sessionId, string imageId)
        {
            var response = await Client.GetAsync($"acquisition/{sessionId}/images/{imageId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<ImageInfo>();
        }

        #endregion
    }
}
