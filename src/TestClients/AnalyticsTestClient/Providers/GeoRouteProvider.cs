// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace AnalyticsTestClient
{
    public class GeoRouteProvider : IGeoRouteProvider
    {
        private string _routesFolder;
        private static string[] _files = null;

        public GeoRouteProvider(string routesFolder)
        {
            _routesFolder = routesFolder;
        }

        public string[] Files
        {
            get
            {
                if (_files == null)
                {
                    _files = Directory.GetFiles(_routesFolder, "*.json", SearchOption.AllDirectories);
                }

                return _files;
            }
        }

        public GeoRoute GetGeoRoute()
        {
            var file = Files.TakeRandom();
            var geoJson = File.ReadAllText(file);
            return GeoRoute.Parse(geoJson);
        }
    }
}