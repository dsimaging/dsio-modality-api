
namespace DSIO.Modality.Api.Sdk.Types.V1
{
    public class AcquisitionInfo
    {
        /// <summary>
        /// Flag to enable or disable acquisition
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Amount in degrees that sensor is rotated 
        /// with respect to the neutral position (cable exits south)
        /// </summary>
        public int Rotation { get; set; }

        /// <summary>
        /// The sensor binning mode for the acquisition
        /// </summary>
        public BinningMode Binning { get; set; }

        /// <summary>
        /// Flag to indicate if a dynamic gamma map should be applied
        /// to the image to optimize contrast. The default value is true.
        /// </summary>
        public bool ApplyLut { get; set; } = true;

        /// <summary>
        /// An arbitrary object used to store custom data relevant
        /// to the client. The client may store and retrieve custom
        /// data related to an exposure in this property. The data
        /// is also available in the ImageInfo object generated
        /// from an exposure.
        /// </summary>
        public object Context { get; set; }
    }
}