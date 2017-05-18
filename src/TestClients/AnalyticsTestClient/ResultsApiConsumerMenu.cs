// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AnalyticsTestClient.Utils;
using System;

namespace AnalyticsTestClient
{
    internal class ResultsApiConsumerMenu : ConsoleMenu
    {
        public ResultsApiConsumerMenu()
        {
            Title = "Nether Analytics Test Client - Results API Consumer";

            MenuItems.Add('1', new ConsoleMenuItem("Get Latest Results (FS)", () => Console.WriteLine("Not implemented yet...")));
        }

        public void GetLatestFromFileSystem()
        {
            //.IResultsReader reader;
        }
    }
}
