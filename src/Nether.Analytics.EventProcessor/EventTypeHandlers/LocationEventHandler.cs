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
        private static ConcurrentDictionary<long, LocationLookupInfo> _cache = new ConcurrentDictionary<long, LocationLookupInfo>();

        ILocationLookupProvider _locationLookupProvider;

        public LocationEventHandler(ILocationLookupProvider locationLookupProvider)
        {
            _locationLookupProvider = locationLookupProvider;
        }

        public LocationLookupInfo LookupGeoHash(long geoHash, int precision)
        {
            if (_cache.TryGetValue(geoHash, out LocationLookupInfo info))
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
            _cache.TryAdd(geoHash, location);

            return location;
        }

    }


}
