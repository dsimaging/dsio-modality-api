namespace DSIO.Modality.Api.Sdk.Types.V1
{
    public class DeviceEventData
    {
        public enum EventAction
        {
            added,
            removed,
            changed
        };

        public EventAction Action { get; set; }

        public DeviceInfo DeviceInfo { get; set; }
    }
}