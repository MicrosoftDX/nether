// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace AnalyticsTestClient
{
    public class GeoRouteProvider : IGeoRouteProvider
    {
        private string _routesFolder;
        private static string[] s_files = null;

        public GeoRouteProvider(string routesFolder)
        {
            _routesFolder = routesFolder;
        }

        public string[] Files
        {
            get
            {
                if (s_files == null)
                {
                    s_files = Directory.GetFiles(_routesFolder, "*.json", SearchOption.AllDirectories);
                }

                return s_files;
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