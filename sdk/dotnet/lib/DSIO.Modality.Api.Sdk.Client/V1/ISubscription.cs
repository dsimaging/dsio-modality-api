namespace DSIO.Modality.Api.Sdk.Client.V1
{
    public interface ISubscription
    {
        /// <summary>
        /// Start listening for event data
        /// </summary>
         void Start();

        /// <summary>
        /// Stop listening for event data
        /// </summary>
         void Stop();
    }
}