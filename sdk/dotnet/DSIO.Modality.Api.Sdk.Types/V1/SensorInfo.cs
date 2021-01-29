namespace DSIO.Modality.Api.Sdk.Types.V1
{
    public class SensorInfo
    {
        /// <summary>
        /// Manufacturer's model name
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Manufacturer's serial number
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Manufacturer's Brand
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Product Family that the sensor belongs to
        /// </summary>
        public string Family { get; set; }

        /// <summary>
        /// Sensor size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Width of the sensor in pixels
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the sensor in pixels
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Flag indicating if the sensor supports Binning
        /// mode changes
        /// </summary>
        public bool SupportsBinning { get; set; }

        /// <summary>
        /// Version of software running on sensor
        /// </summary>
        public string Version { get; set; }
    }
}