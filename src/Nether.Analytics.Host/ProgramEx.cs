// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Nether.Analytics.DataLake;
using Nether.Analytics.EventHubs;
using Nether.Analytics.GeoLocation;
using Nether.Analytics.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Nether.Analytics.Host
{
    public class ProgramEx
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


        private IConfigurationRoot _configuration;

        public ProgramEx()
        {
            SetupConfigurationProviders();
        }

        public async Task RunAsync()
        {
            // Check that all configurations are set before continuing
            var configSet = IsConfigSetup(
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

            if (!configSet)
            {
                // Exiting due to missing configuration
                Console.WriteLine("Press any key to continue");
                Console.ReadKey(true);
                return;
            }

            // Authenticate against Azure AD once and re-use for all needed purposes
            var serviceClientCretentials = await ApplicationTokenProvider.LoginSilentAsync(_configuration[NAH_AAD_Domain],
                new ClientCredential(_configuration[NAH_AAD_ClientId], _configuration[NAH_AAD_ClientSecret]));

            // Setup Listener. This will be the same for all pipelines we are building.
            var listenerConfig = new EventHubsListenerConfiguration
            {
                EventHubConnectionString = _configuration[NAH_EHListener_ConnectionString],
                EventHubPath = _configuration[NAH_EHListener_EventHubPath],
                ConsumerGroupName = _configuration[NAH_EHListener_ConsumerGroup],
                StorageConnectionString = _configuration[NAH_EHListener_StorageConnectionString],
                LeaseContainerName = _configuration[NAH_EHListener_LeaseContainerName]
            };
            var listener = new EventHubsListener(listenerConfig);

            // Setup Message Parser. By default we are using Nether JSON Messages
            // Setting up parser that knows how to parse those messages.
            var parser = new EventHubListenerMessageJsonParser(new ConsoleCorruptMessageHandler()) { AllowDbgEnqueuedTime = true };

            // User a builder to create routing infrastructure for messages and the pipelines
            var builder = new MessageRouterBuilder();

            var filePathAlgorithm = new PipelineDateFilePathAlgorithm(newFileOption: NewFileNameOptions.Every3Hours);

            // Setting up "Geo Clustering Recipe"

            var clusteringSerializer = new CsvOutputFormatter("id", "type", "version", "enqueuedTimeUtc", "gameSession", "lat", "lon", "geoHash", "geoHashPrecision", "geoHashCenterLat", "geoHashCenterLon", "geoHashCenterDist", "rnd");

            builder.Pipeline("clustering")
                .HandlesMessageType("geo-location", "1.0.0")
                .AddHandler(new GeoHashMessageHandler { CalculateGeoHashCenterCoordinates = true })
                .AddHandler(new RandomIntMessageHandler())
                .OutputTo(new ConsoleOutputManager(clusteringSerializer)
                        , new FileOutputManager(clusteringSerializer, filePathAlgorithm, @"../../../USQLDataRoot")
                        , new DataLakeStoreOutputManager(
                            clusteringSerializer,
                            filePathAlgorithm,
                            serviceClientCretentials,
                            _configuration[NAH_Azure_SubscriptionId],
                            _configuration[NAH_Azure_DLSOutputManager_AccountName])
                        );

            // Setting up "Daily Active Users Recipe"

            var dauSerializer = new CsvOutputFormatter("id", "type", "version", "enqueuedTimeUtc", "gameSession", "gamerTag");
            builder.Pipeline("dau")
                .HandlesMessageType("session-start", "1.0.0")
                .OutputTo(new ConsoleOutputManager(dauSerializer)
                        , new FileOutputManager(dauSerializer, filePathAlgorithm, @"../../../USQLDataRoot")
                        , new DataLakeStoreOutputManager(
                            dauSerializer,
                            filePathAlgorithm,
                            serviceClientCretentials,
                            _configuration[NAH_Azure_SubscriptionId],
                            _configuration[NAH_Azure_DLSOutputManager_AccountName])
                        );

            var sessionSerializer = new CsvOutputFormatter("id", "type", "version", "enqueuedTimeUtc", "gameSession");
            builder.Pipeline("sessions")
                .HandlesMessageType("heartbeat", "1.0.0")
                .OutputTo(new ConsoleOutputManager(sessionSerializer)
                , new FileOutputManager(sessionSerializer, filePathAlgorithm, @"../../../USQLDataRoot")
                , new DataLakeStoreOutputManager(
                    sessionSerializer,
                    filePathAlgorithm,
                    serviceClientCretentials,
                    _configuration[NAH_Azure_SubscriptionId],
                    _configuration[NAH_Azure_DLSOutputManager_AccountName])
                );

            builder.DefaultPipeline
                .AddHandler(new RandomIntMessageHandler())
                .OutputTo(new ConsoleOutputManager(new CsvOutputFormatter { IncludeHeaders = false }));

            // Build all pipelines
            var router = builder.Build();

            // Attach the differeing parts of the message processor together
            var messageProcessor = new MessageProcessor<EventHubListenerMessage>(listener, parser, router);

            // The following method will never exit
            await messageProcessor.ProcessAndBlockAsync();
        }

        private void SetupConfigurationProviders()
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

            _configuration = configBuilder.Build();
        }

        private bool IsConfigSetup(params string[] settings)
        {
            const int maxValueLengthPrinted = 100;

            Console.WriteLine("Using the following configuration values:");
            Console.WriteLine();

            var missingSettings = new List<string>();

            foreach (var setting in settings)
            {
                var val = _configuration[setting];

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

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
