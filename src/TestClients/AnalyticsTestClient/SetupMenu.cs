// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using AnalyticsTestClient.Utils;

namespace AnalyticsTestClient
{
    public class SetupMenu : ConsoleMenu
    {
        public SetupMenu()
        {
            Title = "Nether Analytics Test Client - Setup Menu";

            MenuItems.Add('1', new ConsoleMenuItem($"Set Event Hub Connection String", SetEventHubConnectionString));
            MenuItems.Add('2', new ConsoleMenuItem($"Set Event Hub Name/Path", SetEventHubNamePath));
        }

        protected override void PrintFooter()
        {
            ShowCurrentConfig();
        }

        private void SetEventHubConnectionString()
        {
            ConfigCache.EventHubConnectionString = (string)EditProperty("Event Hub Connection String", ConfigCache.EventHubConnectionString, typeof(string));
        }

        private void SetEventHubNamePath()
        {
            ConfigCache.EventHubName = (string)EditProperty("Event Hub Name", ConfigCache.EventHubName, typeof(string));
        }

        public static void ShowCurrentConfig()
        {
            Console.WriteLine();
            Console.WriteLine("Using:");
            Console.WriteLine($"  Event Hub Connection String:");
            ConsoleEx.WriteConnectionString(ConfigCache.EventHubConnectionString, 4);
            Console.WriteLine($"  Event Hub Name/Path: {ConfigCache.EventHubName ?? "not specified"}");
            Console.WriteLine();
        }

       
    }
}