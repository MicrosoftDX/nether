// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Nether.Ingest;
using System;
using System.Collections.Generic;
using System.IO;

namespace Nether.Demo.IngestToSql
{
    public static class Config
    {
        private static IConfigurationRoot s_root;

        public static IConfigurationRoot Root
        {
            get { return s_root; }
        }

        static Config()
        {
            SetupConfigurationProviders();
        }

        // Configuration parameters

        public const string AppSettingsFile = "appsettings.json";
        public const string NAH_EHLISTENER_CONNECTIONSTRING = "NAH_EHLISTENER_CONNECTIONSTRING";
        public const string NAH_EHLISTENER_EVENTHUBPATH = "NAH_EHLISTENER_EVENTHUBPATH";
        public const string NAH_EHLISTENER_CONSUMERGROUP = "NAH_EHLISTENER_CONSUMERGROUP";
        public const string NAH_EHLISTENER_STORAGECONNECTIONSTRING = "NAH_EHLISTENER_STORAGECONNECTIONSTRING";
        public const string NAH_EHLISTENER_LEASECONTAINERNAME = "NAH_EHLISTENER_LEASECONTAINERNAME";
        public const string NAH_AZURE_SQLUTPUTMANAGER_CONNECTIONSTRING = "NAH_AZURE_SQLUTPUTMANAGER_CONNECTIONSTRING";


        private static void SetupConfigurationProviders()
        {
            var defaultConfigValues = new Dictionary<string, string>
            {
                {NAH_EHLISTENER_CONNECTIONSTRING, ""},
                {NAH_EHLISTENER_EVENTHUBPATH, "nether"},
                {NAH_EHLISTENER_CONSUMERGROUP, "$default"},
                {NAH_EHLISTENER_STORAGECONNECTIONSTRING, ""},
                {NAH_EHLISTENER_LEASECONTAINERNAME, "sync"}
            };

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddInMemoryCollection(defaultConfigValues)
                .AddJsonFile(AppSettingsFile, optional: true)
                .AddEnvironmentVariables();

            s_root = configBuilder.Build();
        }

        public static bool Check()
        {
            var settings = new List<string>
            {
                NAH_EHLISTENER_CONNECTIONSTRING,
                NAH_EHLISTENER_EVENTHUBPATH,
                NAH_EHLISTENER_CONSUMERGROUP,
                NAH_EHLISTENER_STORAGECONNECTIONSTRING,
                NAH_EHLISTENER_LEASECONTAINERNAME,
                NAH_AZURE_SQLUTPUTMANAGER_CONNECTIONSTRING
            };

            const int maxValueLengthPrinted = 100;

            Console.WriteLine("Using the following configuration values:");
            Console.WriteLine();

            var missingSettings = new List<string>();

            foreach (var setting in settings)
            {
                var val = Config.Root[setting];

                if (string.IsNullOrWhiteSpace(val))
                {
                    missingSettings.Add(setting);
                }
                else
                {
                    ConsoleEx.Write(ConsoleColor.DarkGray, setting);
                    Console.WriteLine(" : ");
                    ConsoleEx.WriteLine(ConsoleColor.Yellow, "  " + (val.Length < maxValueLengthPrinted ? val : val.Substring(0, maxValueLengthPrinted - 3) + "..."));
                }
            }

            Console.WriteLine();

            if (missingSettings.Count > 0)
            {
                Console.WriteLine("The following setting(s) are missing values:");
                Console.WriteLine();


                foreach (var setting in missingSettings)
                {
                    ConsoleEx.WriteLine(ConsoleColor.Magenta, $"  {setting}");
                }

                Console.WriteLine();
                Console.WriteLine($"Make sure to set all the above configuration parameters in {Config.AppSettingsFile} or using Environment Variables.");
                Console.WriteLine("Then start Nether.Analytics.Host again.");
                Console.WriteLine();

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
