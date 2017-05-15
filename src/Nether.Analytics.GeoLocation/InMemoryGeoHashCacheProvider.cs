// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics.GeoLocation
{
    public class InMemoryGeoHashCacheProvider : IGeoHashCacheProvider
    {
        private int _precision;
        public int Precision
        {
            get { return _precision; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Precision must be > 0");
                _precision = value;
            }
        }

        public BingResult this[long geohash]
        {
            get
            {
                if (!_cache.ContainsKey(geohash))
                {
                    throw new ArgumentException($"Geohash {geohash} not found in cache");
                }
                return _cache[geohash];
            }
        }

        private Dictionary<Int64, BingResult> _cache = new Dictionary<Int64, BingResult>();

        public void AppendToCache(Int64 geohash, BingResult bingResult)
        {
            if (_precision <= 0)
                throw new ArgumentException("Precision must be greater than 0");

            _cache.Add(geohash, bingResult);
        }

        public bool ContainsGeoHash(Int64 geohash)
        {
            if (_precision <= 0)
                throw new ArgumentException("Precision must be greater than 0");

            return _cache.ContainsKey(geohash);
        }
    }
}
