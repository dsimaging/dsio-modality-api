using System;

namespace DSIO.Modality.Api.Sdk.Types.V1
{
    public class ImageInfo
    {
        /// <summary>
        /// Unique Id of the image
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Added timestamp.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Width of the image in pixels
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the image in pixels
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Number of bits representing each pixel 
        /// </summary>
        public int BitsPerPixel { get; set; }

        /// <summary>
        /// Number of bytes required to store each pixel
        /// </summary>
        public int BytesPerPixel { get; set; }

        /// <summary>
        /// Horizontal length of pixel in mm
        /// </summary>
        public double PixelSizeX { get; set; }

        /// <summary>
        /// Vertical length of pixel in mm
        /// </summary>
        public double PixelSizeY { get; set; }

        /// <summary>
        /// Url for a small preview of the image
        /// </summary>
        public string PreviewUrl { get; set; }

        /// <summary>
        /// Url for the full sized image
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Hash of the pixel data located at ImageUrl
        /// </summary>
        public HashInfo PixelHash { get; set; }

        /// <summary>
        /// The DeviceInfo for the device (sensor interface) used to acquire the image
        /// </summary>
        public DeviceInfo DeviceInfo { get; set; }

        /// <summary>
        /// The SensorInfo for the sensor used to acquire the image
        /// </summary>
        public SensorInfo SensorInfo { get; set; }

        /// <summary>
        /// Acquisition info used to acquire the image
        /// </summary>
        public AcquisitionInfo AcquisitionInfo { get; set; }

        /// <summary>
        /// Lut mapping info applied to image
        /// </summary>
        public LutInfo LutInfo { get; set; }

        /// <summary>
        /// Exposure Info of the image used for the exposure meter.
        /// </summary>
        public ExposureInfo ExposureInfo { get; set; }
    }
}