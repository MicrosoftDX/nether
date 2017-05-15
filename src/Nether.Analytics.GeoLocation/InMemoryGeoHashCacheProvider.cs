// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics.GeoLocation
{
    public class InMemoryGeoHashCacheProvider : IGeoHashCacheProvider
    {
        private int precision;
        public int Precision
        {
            get { return precision; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Precision must be > 0");
                precision = value;
            }
        }

        public BingResult this[long geohash]
        {
            get
            {
                if (!cache.ContainsKey(geohash))
                {
                    throw new ArgumentException($"Geohash {geohash} not found in cache");
                }
                return cache[geohash];
            }
        }

        Dictionary<Int64, BingResult> cache = new Dictionary<Int64, BingResult>();

        public void AppendToCache(Int64 geohash, BingResult bingResult)
        {
            if (precision <= 0)
                throw new ArgumentException("Precision must be greater than 0");

            cache.Add(geohash, bingResult);
        }

        public bool ContainsGeoHash(Int64 geohash)
        {
            if (precision <= 0)
                throw new ArgumentException("Precision must be greater than 0");

            return cache.ContainsKey(geohash);
        }
    }
}
