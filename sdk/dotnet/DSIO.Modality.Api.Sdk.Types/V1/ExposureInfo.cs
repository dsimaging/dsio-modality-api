namespace DS.IO.ModalityApi.V1.Types
{
    /// <summary>
    /// ExposureInfo provides data needed by the Exposure Meter feature.
    /// The expected range of exposure values is [Min, Max], although it is
    /// possible for exposure values to exceed this range.
    /// The optimal range for exposure values is [Low, High], while the ideal
    /// exposure value is indicated by Optimum.
    /// </summary>
    public class ExposureInfo
    {
        /// <summary>
        /// Gets or sets the minimum value of the exposure scale
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of the exposure scale
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// Gets or sets the low end of the optimal exposure range
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// Gets or sets the high end of the optimal exposure range
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the optimum exposure value
        /// </summary>
        public double Optimum { get; set; }

        /// <summary>
        /// Gets or sets the exposure value.
        /// </summary>
        public double Value { get; set; }
    }
}