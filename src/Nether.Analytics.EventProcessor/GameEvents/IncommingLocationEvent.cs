// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics.EventProcessor.GameEvents
{
    //TODO: Move file out of here as soon as we find a good way of sharing the Game Event Types between different projects.
    // Right now this is a copy of how the event type look like in the Nether.Analytics.GameEvents 
    public class IncommingLocationEvent
    {
        public string Type => "location";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
