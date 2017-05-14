// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Analytics.EventHubs
{
    public class EventHubsListenerConfiguration
    {
        /// <summary>
        /// Gets or sets the event hub path.
        /// </summary>
        /// <value>
        /// The event hub path.
        /// </value>
        public string EventHubPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the consumer group. If left empty, the default consumer group will be used.
        /// </summary>
        /// <value>
        /// The name of the consumer group.
        /// </value>
        public string ConsumerGroupName { get; set; }

        /// <summary>
        /// Gets or sets the event hub connection string.
        /// </summary>
        /// <value>
        /// The event hub connection string.
        /// </value>
        public string EventHubConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the storage connection string. EventHubListener uses this storage account
        /// to synchronize reads among threads.
        /// </summary>
        /// <value>
        /// The storage connection string.
        /// </value>
        public string StorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the lease container.
        /// </summary>
        /// <value>
        /// The name of the lease container used to synchronize reads among threads.
        /// </value>
        public string LeaseContainerName { get; set; }
    }
}
