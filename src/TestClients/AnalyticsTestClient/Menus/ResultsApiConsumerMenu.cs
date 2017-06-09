// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            MenuItems.Add('2', new ConsoleMenuItem("Time Span Results (FS)", () => new ResultsTimeSpanMenu().Show()));
        }

        private class ResultsTimeSpanMenu : ConsoleMenu
        {
            public ResultsTimeSpanMenu()
            {
                Title = "Latest Results (FS)";

                var clusteringSerializer = new CsvMessageFormatter("id", "type", "version", "enqueueTimeUtc", "gameSession", "lat", "lon", "geoHash", "geoHashPrecision", "geoHashCenterLat", "geoHashCenterLon", "rnd");
                var dauSerializer = new CsvMessageFormatter("id", "type", "version", "enqueueTimeUtc", "gameSession", "gamerTag");


                MenuItems.Add('1', new ConsoleMenuItem("DAU", () => GetResults(dauSerializer, "dau", "session-start")));
                MenuItems.Add('2', new ConsoleMenuItem("Clustering", () => GetResults(clusteringSerializer, "clustering", "geo-location")));
            }

            private void GetResults(IMessageFormatter formatter, string pipeline, string messageType)
            {
                var start = GetDate("Start date");
                var end = GetDate("End date");

                if (start == null || end == null) return;

                var filePathAlgorithm = new DateFolderStructure(newFileOption: NewFileNameOptions.Every5Minutes);

                IFolderStructure _filePathAlgorithm = new DateFolderStructure(newFileOption: NewFileNameOptions.Every5Minutes);

                Console.Write("Root directory: ");

                var path = Console.ReadLine();

                var f = new Nether.Analytics.FileResultsReader(formatter, _filePathAlgorithm, path, pipeline, messageType);
                foreach (var msg in f.Get(start.Value, end.Value))
                {
                    Console.WriteLine(msg.ToString());
                }
            }

            private DateTime? GetDate(string type)
            {
                Console.Write($"{type}:");
                var d = Console.ReadLine();

                DateTime date;
                if (!DateTime.TryParse(d, out date))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Date was incorrectly formatted.");
                    Console.ResetColor();
                    return null;
                }

                return date;
            }
        }


        private class ResultsFileSystemMenu : ConsoleMenu
        {
            public ResultsFileSystemMenu()
            {
                Title = "Latest Results (FS)";

                var dauSerializer = new CsvMessageFormatter("id", "type", "version", "enqueuedTimeUtc", "gameSession", "gamerTag");
                var clusteringSerializer = new CsvMessageFormatter("id", "type", "version", "enqueueTimeUtc", "gameSession", "lat", "lon", "geoHash", "geoHashPrecision", "geoHashCenterLat", "geoHashCenterLon", "rnd");


                MenuItems.Add('1', new ConsoleMenuItem("DAU", () => GetLatestFromFileSystem(dauSerializer, "dau", "session-start")));
                MenuItems.Add('2', new ConsoleMenuItem("Clustering", () => GetLatestFromFileSystem(clusteringSerializer, "clustering", "geo-location")));
            }

            public void GetLatestFromFileSystem(IMessageFormatter formatter, string pipeline, string messageType)
            {
                IFolderStructure _filePathAlgorithm = new DateFolderStructure(newFileOption: NewFileNameOptions.Every5Minutes);

                Console.Write("Root directory: ");

                var path = Console.ReadLine();

                var f = new Nether.Analytics.FileResultsReader(formatter, _filePathAlgorithm, path, pipeline, messageType);

                var messages = f.GetLatest();

                foreach (var msg in messages)
                {
                    Console.WriteLine(msg.ToString());
                }
            }
        }
    }
}
