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

            MenuItems.Add('1', new ConsoleMenuItem("Get Latest Results (FS)", () => new ResultsFileSystemMenu().Show()));
        }

        private class ResultsFileSystemMenu : ConsoleMenu
        {
            public ResultsFileSystemMenu()
            {
                Title = "Latest Results (FS)";

                var clusteringSerializer = new CsvMessageFormatter("id", "type", "version", "enqueueTimeUtc", "gameSession", "lat", "lon", "geoHash", "geoHashPrecision", "geoHashCenterLat", "geoHashCenterLon", "rnd");
                var dauSerializer = new CsvMessageFormatter("id", "type", "version", "gameSession", "enqueueTimeUtc", "gamerTag");


                MenuItems.Add('1', new ConsoleMenuItem("DAU Serializer", () => GetLatestFromFileSystem(dauSerializer)));
                MenuItems.Add('2', new ConsoleMenuItem("Clustering Serliazier", () => GetLatestFromFileSystem(clusteringSerializer)));
            }
            
            public void GetLatestFromFileSystem(IMessageFormatter formatter)
            {
                Console.Write("Root directory: ");

                var path = Console.ReadLine();

                var f = new Nether.Analytics.FileResultsReader(formatter, null, path);

                var messages = f.GetLatest();

                foreach (var msg in messages)
                {
                    Console.WriteLine(msg.ToString());
                }
            }
        }

        
    }
}
