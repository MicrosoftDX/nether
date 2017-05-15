// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics.GeoLocation
{
    public interface IGeoHashCacheProvider
    {
        void AppendToCache(Int64 geohash, BingResult bingResult);
        bool ContainsGeoHash(Int64 geohash);
        int Precision { get; set; }
        BingResult this[Int64 geohash] { get; }
    }
}
