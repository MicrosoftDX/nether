// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NGeoHash.Portable;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nether.Analytics.EventProcessor.EventTypeHandlers
{
    public class LocationEventHandler
    {
        private static ConcurrentDictionary<long, LocationLookupInfo> s_cache = new ConcurrentDictionary<long, LocationLookupInfo>();

        private ILocationLookupProvider _locationLookupProvider;

        public LocationEventHandler(ILocationLookupProvider locationLookupProvider)
        {
            _locationLookupProvider = locationLookupProvider;
        }

        public LocationLookupInfo LookupGeoHash(long geoHash, int precision)
        {
            if (s_cache.TryGetValue(geoHash, out LocationLookupInfo info))
            {
                Console.WriteLine($"Cached geohash {geoHash}:{precision} is in {info.City}, {info.District}, {info.Country}");
                return info;
            }

            Console.WriteLine($"Looking up geohash {geoHash}:{precision}");
            var geoHashCenter = GeoHash.DecodeInt(geoHash, precision).Coordinates;
            var l = _locationLookupProvider.Lookup(geoHashCenter.Lat, geoHashCenter.Lon);
            l.Wait();
            var location = l.Result;
            Console.WriteLine($"Geohash {geoHash}:{precision} is in {location.City}, {location.District}, {location.Country} according to {_locationLookupProvider.GetType()}");
            s_cache.TryAdd(geoHash, location);

            return location;
        }
    }
}
