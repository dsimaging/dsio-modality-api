﻿using System;

namespace DS.IO.ModalityApi.V1.Types
{
    public class AcquisitionSession
    {
        /// <summary>
        /// Unique Id of the Acquisition Session
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Unique Id of the device used for a session
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Identifying name of the client application that can be used
        /// to indicate the client that created a session
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Added timestamp.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// An arbitrary object used to store custom data relevant
        /// to the client. The client may store and retrieve custom
        /// data related to an acquisition session.
        /// </summary>
        public object Context { get; set; }
    }
}
