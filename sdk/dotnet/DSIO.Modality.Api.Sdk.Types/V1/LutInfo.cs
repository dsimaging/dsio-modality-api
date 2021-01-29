namespace DS.IO.ModalityApi.V1.Types
{
    /// <summary>
    /// Describes the lookup table (LUT) mapping applied to an image. 
    /// Images acquired by this service may be mapped using a non-linear LUT 
    /// in order to provide an image suitable for presentation. 
    /// 
    /// The LUT is created according to the following equation:
    ///
    /// Y = m(x′γ) + b
    /// where
    ///     m is the slope
    ///     b is the offset
    ///     γ is the gamma value
    ///     x′ is a normalized pixel scaled to the range[0, 1]
    ///     
    /// If desired, the original pixel data(X) can be recovered from the mapped pixel(Y) 
    /// using the LutInfo:
    ///
    /// X = (Xmax - Xmin) *[(Y - b) / m]1/γ + Xmin
    /// where
    ///     m is the slope
    ///     b is the offset
    ///     γ is the gamma value
    ///     Xmin is the minimum gray value
    ///     Xmax is the maximum gray value
    /// </summary>
    public class LutInfo
    {
        /// <summary>
        /// Gamma value used in map
        /// </summary>
        public double Gamma { get; set; }

        /// <summary>
        /// Slope value used in map
        /// </summary>
        public double Slope { get; set; }

        /// <summary>
        /// Offset value used in map
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// Total number of possibe gray levels (full range) in original unmapped image
        /// </summary>
        public int TotalGrays { get; set; }

        /// <summary>
        /// Minimum value of gray level present in original unmapped image
        /// </summary>
        public int MinimumGray { get; set; }

        /// <summary>
        /// Maximum value of gray level present in original unmapped image
        /// </summary>
        public int MaximumGray { get; set; }
    }
}
