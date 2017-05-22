// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AnalyticsTestClient.Utils;
using Nether.Analytics;
using System;

namespace AnalyticsTestClient
{
    internal class ResultsApiConsumerMenu : ConsoleMenu
    {
        public ResultsApiConsumerMenu()
        {
            Title = "Nether Analytics Test Client - Results API Consumer";

            MenuItems.Add('1', new ConsoleMenuItem("Get Latest Results (FS)", () => GetLatestFromFileSystem()));
        }

        public void GetLatestFromFileSystem()
        {
            var clusteringSerializer = new CsvMessageFormatter("id", "type", "version", "enqueueTimeUtc", "gameSession", "lat", "lon", "geoHash", "geoHashPrecision", "geoHashCenterLat", "geoHashCenterLon", "rnd");
            var dauSerializer = new CsvMessageFormatter("id", "type", "version", "gameSession", "enqueueTimeUtc", "gamerTag");

            var f = new Nether.Analytics.FileResultsReader(clusteringSerializer, null, @"C:\Users\anvod\Documents\GitHub\nether-nonversioncontroller\USQLDataRoot\nether\clustering\geo-location");

            var messages = f.GetLatest();

            foreach (var msg in messages)
            {
                Console.WriteLine(msg.ToString());
            }
        }
    }
}
