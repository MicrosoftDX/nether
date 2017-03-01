// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics.EventProcessor.GameEvents
{
    public class OutgoingLocationEvent : IOutGameEvent
    {
        public string Type => "location";
        public string Version => "1.0.0";
        public DateTime EnqueueTime { get; set; }
        public DateTime DequeueTime { get; set; }
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public long GeoHash { get; set; }
        public int GeoHashPrecision { get; set; }
        public double GeoHashCenterLat { get; set; }
        public double GeoHashCenterLon { get; set; }
        public string Country { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
