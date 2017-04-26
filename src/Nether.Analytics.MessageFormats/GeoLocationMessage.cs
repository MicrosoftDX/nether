// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics.MessageFormats
{
    public class LocationMessage : INetherMessage
    {
        public string Type => "geo-location";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}