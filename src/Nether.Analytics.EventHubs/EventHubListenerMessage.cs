// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;

namespace Nether.Analytics.EventHubs
{
    public class EventHubListenerMessage
    {
        public ArraySegment<byte> Body { get; set; }
        public DateTime EnqueuedTime { get; set; }
        public DateTime DequeuedTime { get; set; }
        public string PartitionId { get; set; }
        public long SequenceNumber { get; set; }
    }
}