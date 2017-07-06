// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GeoCoordinatePortable;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nether.Test.ConsoleClient
{
    public class GeoRoute
    {
        private List<RouteSection> _routeSections;

        public GeoRoute(List<RouteSection> routeSections)
        {
            _routeSections = routeSections;
        }

        public static GeoRoute Parse(string geoJsonFeatureCollection)
        {
            var routeSections = new List<RouteSection>();
            var lastDistance = 0f;
            var lastTime = TimeSpan.Zero;

            var featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(geoJsonFeatureCollection);


            foreach (var feature in featureCollection.Features)
            {
                if (feature.Type == GeoJSON.Net.GeoJSONObjectType.Feature && feature.Geometry.Type == GeoJSON.Net.GeoJSONObjectType.LineString)
                {
                    var section = new RouteSection();

                    section.FromDistance = lastDistance;
                    section.FromTime = lastTime;

                    if (feature.Properties.TryGetValue("name", out var name))
                        section.Name = (string)name;
                    else
                        section.Name = "N/A";

                    section.ToDistance = float.Parse((string)feature.Properties["distance"]);
                    section.ToTime = TimeSpan.FromSeconds(double.Parse((string)feature.Properties["time"]));

                    var lineString = feature.Geometry as LineString;
                    foreach (var coordinate in lineString.Coordinates)
                    {
                        var position = coordinate as GeographicPosition;
                        section.Coordinates.Add(new GeoCoordinate(position.Latitude, position.Longitude));
                    }

                    routeSections.Add(section);

                    lastDistance = section.ToDistance;
                    lastTime = section.ToTime;
                }
            }

            return new GeoRoute(routeSections);
        }

        public GeoCoordinate GetPosition(TimeSpan at)
        {
            var section = (from s in _routeSections
                           where s.FromTime <= at && at < s.ToTime
                           select s).FirstOrDefault();

            // If no section was found then we are at the end of the route
            if (section == null)
                return null;

            // If section by some reason onlycontains 1 coordinate, return that one, no matter what time it is
            if (section.Coordinates.Count == 1)
                return section.Coordinates.First();

            // Assuming that all coordinates within a route section is spaced evenly
            //TODO: Make even more clever calculations using the distance between coordinates instead of the above assumption

            var timeIntoSection = at - section.FromTime;
            var totalSectionTime = section.ToTime - section.FromTime;
            var percentageIntoSection = timeIntoSection.TotalSeconds / totalSectionTime.TotalSeconds;
            var noCoordinateSections = section.Coordinates.Count - 1;
            var decimalPositionInCoordinatesList = noCoordinateSections * percentageIntoSection;

            int i = (int)Math.Floor(decimalPositionInCoordinatesList);
            int j = (int)Math.Ceiling(decimalPositionInCoordinatesList);

            var c1 = section.Coordinates[i];
            var c2 = section.Coordinates[j];

            if (c1 == c2)
                return c1;

            var pairStartPercentage = (double)i / noCoordinateSections;
            var pairStopPercentage = (double)j / noCoordinateSections;

            var percentageIntoPair = (percentageIntoSection - pairStartPercentage) / (pairStopPercentage - pairStartPercentage);

            var lat = c1.Latitude + (c2.Latitude - c1.Latitude) * percentageIntoPair;
            var lon = c1.Longitude + (c2.Longitude - c1.Longitude) * percentageIntoPair;

            return new GeoCoordinate(lat, lon);
        }

        public class RouteSection
        {
            public List<GeoCoordinate> Coordinates { get; set; } = new List<GeoCoordinate>();

            public float FromDistance { get; set; }
            public float ToDistance { get; set; }

            public TimeSpan FromTime { get; set; }
            public TimeSpan ToTime { get; set; }

            public string Name { get; set; }
        }
    }
}
