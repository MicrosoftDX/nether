// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Analytics.MessageFormats
{
    public class GeoLocation : ITypeVersionMessage
    {
        public string Type => "geo-location";
        public string Version => "1.0.0";
        public string GameSession { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}