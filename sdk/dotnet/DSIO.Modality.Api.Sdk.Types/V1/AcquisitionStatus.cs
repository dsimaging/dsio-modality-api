namespace DS.IO.ModalityApi.V1.Types
{
    public class AcquisitionStatus
    {
        public enum AcquisitionState
        {
            /// <summary>
            /// General or unknown error
            /// </summary>
            Error,

            /// <summary>
            /// Not enough battery power to acquire an image
            /// </summary>
            LowBattery,

            /// <summary>
            /// Not enough space to store image data
            /// </summary>
            InsufficientStorage,

            /// <summary>
            /// No device is connected
            /// </summary>
            NoHardware,

            /// <summary>
            /// No sensor is connected
            /// </summary>
            NoSensor,

            /// <summary>
            /// The device is initializing
            /// </summary>
            Initializing,

            /// <summary>
            /// No Acquisition Info has been supplied
            /// </summary>
            NoAcquisitionInfo,

            /// <summary>
            /// Ready and waiting for an exposure
            /// </summary>
            Ready,

            /// <summary>
            /// Reading image from the device
            /// </summary>
            Reading,

            /// <summary>
            /// Processing image
            /// </summary>
            Processing,

            /// <summary>
            /// Storing image
            /// </summary>
            Storing,

            /// <summary>
            /// A new image is available
            /// </summary>
            NewImage
        }

        /// <summary>
        /// Indicates exposure readiness of the sensor.
        /// </summary>
        public bool Ready { get; set; }

        /// <summary>
        /// The state of the current exposure
        /// </summary>
        public AcquisitionState State { get; set; }

        /// <summary>
        /// The id of the most recent image acquired.
        /// </summary>
        public string LastImageId { get; set; }

        /// <summary>
        /// Number of images acquired in session
        /// </summary>
        public int TotalImages { get; set; }
    }
}