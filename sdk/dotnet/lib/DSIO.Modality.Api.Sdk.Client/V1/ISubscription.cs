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
        /// The OnStarted event is sent once when the subscription is started
        /// </summary>
        event Action OnStarted;

        /// <summary>
        /// The OnStopped event is sent once when the subscription is stopped
        /// </summary>
        event Action OnStopped;

        /// <summary>
        /// The OnError event is fired when an unexpected exception occurs while
        /// processing or handling the subscirpiton.
        /// </summary>
        event Action<Exception> OnError;

        /// <summary>
        /// The OnHeartbeat event is sent for every heartbeat received from the server.
        /// This event is useful if you wish to monitor the state of the subscription.
        /// </summary>
        event Action<Heartbeat> OnHeartbeat;
    }
}