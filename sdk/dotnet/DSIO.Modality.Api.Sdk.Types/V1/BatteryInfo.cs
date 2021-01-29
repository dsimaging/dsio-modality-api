namespace DSIO.Modality.Api.Sdk.Types.V1
{
    public enum BatteryLevel
    {
        /// <summary>
        /// The battery was recently recharged
        /// </summary>
        Full,

        /// <summary>
        /// The battery level is sufficient
        /// </summary>
        Good,

        /// <summary>
        /// The battery is low and should be recharged
        /// </summary>
        Low
    }

    public class BatteryInfo
    {
        /// <summary>
        /// Flag indicating if the device is battery powered.
        /// </summary>
        public bool HasBattery { get; set; }

        /// <summary>
        /// A number in the range 0-100 indicating the percentage of battery power remaining
        /// </summary>
        public double PercentRemaining { get; set; }

        /// <summary>
        /// A coarse description of the battery power level.
        /// May be used to update a color scheme in the user interface.
        /// </summary>
        public BatteryLevel Level { get; set; }
    }
}