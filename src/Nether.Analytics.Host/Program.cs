// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Nether.Analytics.Bing;
using Nether.Analytics.DataLake;
using Nether.Analytics.EventHubs;
using Nether.Analytics.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nether.Analytics.Host
{
    internal class Program
    {
        private const string AppSettingsFile = "appsettings.json";

        // Configuration parameters
        private const string NAH_EHListener_ConnectionString = "NAH_EHLISTENER_CONNECTIONSTRING";
        private const string NAH_EHListener_EventHubPath = "NAH_EHLISTENER_EVENTHUBPATH";
        private const string NAH_EHListener_ConsumerGroup = "NAH_EHLISTENER_CONSUMERGROUP";
        private const string NAH_EHListener_StorageConnectionString = "NAH_EHLISTENER_STORAGECONNECTIONSTRING";
        private const string NAH_EHListener_LeaseContainerName = "NAH_EHLISTENER_LEASECONTAINERNAME";

        private const string NAH_AAD_Domain = "NAH_AAD_DOMAIN";
        private const string NAH_AAD_ClientId = "NAH_AAD_CLIENTID";
        private const string NAH_AAD_ClientSecret = "NAH_AAD_CLIENTSECRET";

        private const string NAH_Azure_SubscriptionId = "NAH_AZURE_SUBSCRIPTIONID";

        private const string NAH_Azure_DLSOutputManager_AccountName = "NAH_AZURE_DLSOUTPUTMANAGER_ACCOUNTNAME";


        private static IConfigurationRoot Configuration;

        private static void Main(string[] args)
        {
            Greet();

            SetupConfigurationProviders();

            var configStatus = CheckConfigurationStatus(
                NAH_EHListener_ConnectionString,
                NAH_EHListener_EventHubPath,
                NAH_EHListener_ConsumerGroup,
                NAH_EHListener_StorageConnectionString,
                NAH_EHListener_LeaseContainerName,
                NAH_AAD_Domain,
                NAH_AAD_ClientId,
                NAH_AAD_ClientSecret,
                NAH_Azure_SubscriptionId,
                NAH_Azure_DLSOutputManager_AccountName);

            if (configStatus != ConfigurationStatus.Ok)
            {
                // Exiting due to missing configuration
                Console.WriteLine("Press any key to continue");
                Console.ReadKey(true);
                return;
            }

            // Setup Listener
            var listenerConfig = new EventHubsListenerConfiguration
            {
                EventHubConnectionString = Configuration[NAH_EHListener_ConnectionString],
                EventHubPath = Configuration[NAH_EHListener_EventHubPath],
                ConsumerGroupName = Configuration[NAH_EHListener_ConsumerGroup],
                StorageConnectionString = Configuration[NAH_EHListener_StorageConnectionString],
                LeaseContainerName = Configuration[NAH_EHListener_LeaseContainerName]
            };

            var listener = new EventHubsListener(listenerConfig);

            // Setup Message Parser
            var parser = new EventHubJsonMessageParser();

            // Setup Output Managers
            //var blobOutputManager = new BlobOutputManager(outputblobStorageConnectionString);
            //var eventHubOutputManager = new EventHubOutputManager(outputEventHubConnectionString);
            var consoleOutputManager = new ConsoleOutputManager(new MessageJsonSerializer());
            var consoleOutputManager2 = new ConsoleOutputManager(new MessageCsvSerializer());
            var dlsOutputManager = new DataLakeStoreOutputManager(
                domain: Configuration[NAH_AAD_Domain],
                clientId: Configuration[NAH_AAD_ClientId],
                clientSecret: Configuration[NAH_AAD_ClientSecret],
                subscriptionId: Configuration[NAH_Azure_SubscriptionId],
                adlsAccountName: Configuration[NAH_Azure_DLSOutputManager_AccountName]);


            // Build up the Router Pipeline
            var builder = new MessageRouterBuilder();

            builder.AddMessageHandler(new GamerInfoEnricher());
            builder.UnhandledEvent().OutputTo(consoleOutputManager);

            builder.Event("geo-location|1.0.0")
                .AddHandler(new NullMessageHandler())
                .OutputTo(consoleOutputManager, dlsOutputManager);

            var router = builder.Build();

            var messageProcessor = new MessageProcessor<EventHubJsonMessage>(listener, parser, router);


            // Run in an async context since main method is not allowed to be marked as async
            Task.Run(async () =>
            {
                await messageProcessor.ProcessAndBlockAsync();
            }).GetAwaiter().GetResult();
        }


        private static void SetupConfigurationProviders()
        {
            var defaultConfigValues = new Dictionary<string, string>
            {
                {NAH_EHListener_ConnectionString, ""},
                {NAH_EHListener_EventHubPath, "nether"},
                {NAH_EHListener_ConsumerGroup, "$default"},
                {NAH_EHListener_StorageConnectionString, ""},
                {NAH_EHListener_LeaseContainerName, "sync"}
            };

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddInMemoryCollection(defaultConfigValues)
                .AddJsonFile(AppSettingsFile, optional: true)
                .AddEnvironmentVariables();

            Configuration = configBuilder.Build();
        }

        private static ConfigurationStatus CheckConfigurationStatus(params string[] settings)
        {
            const int maxValueLengthPrinted = 100;

            Console.WriteLine("Using the following configuration values:");
            Console.WriteLine();

            var missingSettings = new List<string>();

            foreach (var setting in settings)
            {
                var val = Configuration[setting];

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
                Console.WriteLine($"Make sure to set all the above configuration parameters in {AppSettingsFile} or using Environment Variables.");
                Console.WriteLine("Then start Nether.Analytics.Host again.");
                Console.WriteLine();

                return ConfigurationStatus.MissingConfig;
            }
            else
            {
                return ConfigurationStatus.Ok;
            }
        }

        private static void Greet()
        {
            Console.WriteLine();
            Console.WriteLine(@"   _   _      _   _               ");
            Console.WriteLine(@"  | \ | | ___| |_| |__   ___ _ __ ");
            Console.WriteLine(@"  |  \| |/ _ \ __| '_ \ / _ \ '__|");
            Console.WriteLine(@"  | |\  |  __/ |_| | | |  __/ |   ");
            Console.WriteLine(@"  |_| \_|\___|\__|_| |_|\___|_| Analytics");
            Console.WriteLine(@"  Message Processor Host ");
            Console.WriteLine();
        }
    }

    public enum ConfigurationStatus
    {
        Ok,
        MissingConfig
    }

    public static class ConsoleEx
    {
        public static void Write(ConsoleColor color, string value)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(value);
            Console.ForegroundColor = currentColor;
        }

        public static void WriteLine(ConsoleColor color, string value)
        {
            Write(color, value + Environment.NewLine);
        }
    }
}
