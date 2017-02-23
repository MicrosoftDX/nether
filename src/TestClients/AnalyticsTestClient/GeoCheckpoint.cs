// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace AnalyticsTestClient
{
    public class GeoCheckpoint
    {
        GeoPosition _position;
        TimeSpan _atTime;

        public GeoCheckpoint(GeoPosition position, TimeSpan atTime)
        {
            _position = position;
            _atTime = atTime;
        }

        public GeoCheckpoint(double lat, double lon, TimeSpan atTime) :
            this(new GeoPosition(lat, lon), atTime)
        {
        }

        public GeoCheckpoint(double lat, double lon, int atTimeInSeconds) :
            this(new GeoPosition(lat, lon), TimeSpan.FromSeconds(atTimeInSeconds))
        {
        }

        public GeoCheckpoint(double lat, double lon) :
            this(new GeoPosition(lat, lon), TimeSpan.FromSeconds(0))
        {
        }

        public GeoPosition Position => _position;
        public TimeSpan AtTime => _atTime;
    }
    
}