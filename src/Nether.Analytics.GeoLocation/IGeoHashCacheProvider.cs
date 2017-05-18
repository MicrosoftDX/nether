// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics.GeoLocation
{
    public interface IGeoHashCacheProvider
    {
        Task AppendToCacheAsync(Int64 geohash, BingResult bingResult);
        Task<bool> ContainsGeoHashAsync(Int64 geohash);
        int Precision { get; set; }
        Task<BingResult> GetAsync(Int64 geohash);
    }
}
