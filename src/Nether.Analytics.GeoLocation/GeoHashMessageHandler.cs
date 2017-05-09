// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NGeoHash;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics.GeoLocation
{
    public class GeoHashMessageHandler : IMessageHandler
    {
        public string InputLatProperty { get; set; } = "lat";
        public string InputLonProperty { get; set; } = "lon";
        public string GeoHashProperty { get; set; } = "geoHash";
        public int GeoHashPrecision { get; set; } = 32;
        public bool CalculateGeoHashCenterCoordinates { get; set; } = true;
        public string GeoHashCenterLatProperty { get; set; } = "geoHashCenterLat";
        public string GeoHashCenterLonProperty { get; set; } = "geoHashCenterLon";

        public async Task<MessageHandlerResluts> ProcessMessageAsync(Message msg, string pipelineName, int idx)
        {
            //TODO: Handle incorrect messages that doesn't have the required properties
            var lat = double.Parse(msg.Properties[InputLatProperty]);
            var lon = double.Parse(msg.Properties[InputLonProperty]);

            var precision = GeoHashPrecision;
            var geoHash = GeoHash.EncodeInt(lat, lon, precision);

            msg.Properties[GeoHashProperty] = geoHash.ToString();
            msg.Properties[GeoHashProperty + "Precision"] = precision.ToString();

            if (CalculateGeoHashCenterCoordinates)
            {
                var x = GeoHash.DecodeInt(geoHash, precision);

                msg.Properties[GeoHashCenterLatProperty] = x.Coordinates.Lat.ToString();
                msg.Properties[GeoHashCenterLonProperty] = x.Coordinates.Lon.ToString();
            }

            return MessageHandlerResluts.Success;
        }
    }
}
