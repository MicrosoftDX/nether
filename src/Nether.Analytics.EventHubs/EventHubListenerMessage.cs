// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;

namespace Nether.Analytics.EventHubs
{
    public class EventHubJsonMessage
    {
        public string MessageType { get; set; }
        public ArraySegment<byte> Body { get; internal set; }
        public DateTime EnqueuedTime { get; internal set; }
        public DateTime DequeuedTime { get; internal set; }
    }
}