using System;
using DSIO.Modality.Api.Sdk.Types.V1;

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

        /// <summary>
        /// An event that is triggered for every heartbeat.
        /// </summary>
         event Action<Heartbeat> OnHeartbeat;
    }
}