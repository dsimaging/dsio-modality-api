
namespace DSIO.Modality.Api.Sdk.Types.V1
{
    public class DeviceInfo
    {
        /// <summary>
        /// Unique Id of this device instance
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// User friendly name of the device
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Url for an icon used to represent device
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Flag indicating if the device has a sensor connected
        /// </summary>
        public bool HasSensor { get; set; }

        /// <summary>
        /// Status of device indicating availability for use
        /// </summary>
        public DeviceStatus Status { get; set; }

        /// <summary>
        /// The interface used to connect the device to the system (usb, network...)
        /// </summary>
        public string InterfaceType { get; set; }

        /// <summary>
        /// Manufacturer's model name
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Manufacturer's serial number
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Primary version of device
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The battery info of the device
        /// </summary>        
        public BatteryInfo Battery { get; set; }

    }

}