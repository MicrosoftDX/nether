// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace AnalyticsTestClient.Simulations
{
    public class GeoPosition
    {
        double _lat;
        double _lon;
        long _geoHash;
        int _precision;

        public GeoPosition(double  lat, double lon)
        {
            _lat = lat;
            _lon = lon;

            var precision = 32;
            _geoHash = NGeoHash.GeoHash.EncodeInt(lat, lon, precision);
            _precision = precision;
        }

        public double Lat => _lat;
        public double Lon => _lon;
        public long GeoHash => _geoHash;
        public int Precision => _precision;
    }
    
}