// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics.GameEvents
{
    //TODO: Fix duplication of type LocationEvent, see below
    // WARNING!!!
    // THIS TYPE IS DUPLICATED AND EXISTS INSIDE FILE GameEventHandler.cs as well
    // This is due to an issue where we are unable to reference to the GameEvents project
    // from a native .NET Client (such as the GameEventsProcessor is)
    // REMEMBER TO UPDATE THAT OTHER CLASS AS WELL IF YOU DO ANY UPDATES
    public class LocationEvent : IGameEvent
    {
        public string Type => "location";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
        public double Longitude { get; set; }
        public double Latitute { get; set; }
        public long Geohash { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}