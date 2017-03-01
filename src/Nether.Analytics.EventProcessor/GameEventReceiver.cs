// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Azure.WebJobs.ServiceBus;
using Nether.Analytics.EventProcessor.Output.Blob;
using Nether.Analytics.EventProcessor.Output.EventHub;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;
using System.Threading;
using Nether.Analytics.EventProcessor.EventTypeHandlers;

namespace Nether.Analytics.EventProcessor
{
    /// <summary>
    /// Main class of the EventProcessor. This class has the required trigger(s) to
    /// get called by the WebJob SDK whenever there is a new Event to Process
    /// </summary>
    public static class GameEventReceiver
    {
        private static GameEventRouter s_router;
        private const string FullMessagesQueueName = "fullmessages";
        private static BlobOutputManager s_blobOutputManager;

        private static object s_initializationLock = new object();
        private static Task s_initializationTask;

        /// <summary>
        /// Initialize the GameEventReceiver. Initialization is complete when the returned Task completes.
        /// Safe to call multiple times.
        /// </summary>
        /// <returns></returns>
        private static Task InitializeGameEventReceiverAsync()
        {
            /// double-check lock for perf
            if (s_initializationTask != null)
            {
                return s_initializationTask;
            }
            lock (s_initializationLock)
            {
                if (s_initializationTask == null)
                {
                    s_initializationTask = InitializeInternal();
                }
                return s_initializationTask;
            }
            async Task InitializeInternal()
            {
                // Read Environment Variables for configuration
                // TODO: Fix configuration to be in line with other configuration in Nether

                Console.WriteLine("Configuring GameEventReceiver (from Environment Variables");

                var storageAccountConnectionString = ConfigResolver.Resolve("NETHER_ANALYTICS_STORAGE_CONNECTIONSTRING");
                Console.WriteLine($"outputStorageAccountConnectionString: {storageAccountConnectionString}");

                var outputContainer = ConfigResolver.Resolve("NETHER_ANALYTICS_STORAGE_CONTAINER");
                if (string.IsNullOrWhiteSpace(outputContainer))
                    outputContainer = "gameevents";
                Console.WriteLine($"outputContainer: {outputContainer}");

                var tmpContainer = ConfigResolver.Resolve("NETHER_ANALYTICS_STORAGE_TMP_CONTAINER");
                if (string.IsNullOrWhiteSpace(tmpContainer))
                    tmpContainer = "tmp";
                Console.WriteLine($"tmpContainer: {tmpContainer}");

                var outputEventHubConnectionString = ConfigResolver.Resolve("NETHER_INTERMEDIATE_EVENTHUB_CONNECTIONSTRING");
                Console.WriteLine($"outputEventHubConnectionString: {outputEventHubConnectionString}");

                var outputEventHubName = ConfigResolver.Resolve("NETHER_INTERMEDIATE_EVENTHUB_NAME");
                Console.WriteLine($"outputEventHubName: {outputEventHubName}");

                string webJobDashboardAndStorageConnectionString = ConfigResolver.Resolve("NETHER_WEBJOB_DASHBOARD_AND_STORAGE_CONNECTIONSTRING");

                var maxBlobSize = 100 * 1024 * 1024; // 100MB, USE CONFIG TO CHANGE MAX SIZE
                var maxBlobSizeConfig = ConfigResolver.Resolve("NETHER_BLOB_MAX_SIZE");
                if (!string.IsNullOrWhiteSpace(maxBlobSizeConfig))
                    maxBlobSize = int.Parse(maxBlobSizeConfig);


                Console.WriteLine($"Max Blob Size: {maxBlobSize / 1024 / 1024}MB ({maxBlobSize}B)");

                var bingMapsKey = ConfigResolver.Resolve("NETHER_BING_MAPS_KEY");
                if (string.IsNullOrWhiteSpace(bingMapsKey))
                    Console.WriteLine("Location lookup is not configured, please specify a configuration for NETHER_BING_MAPS_KEY");
                else
                    Console.WriteLine($"Using Bing Maps to lookup locations with key: {bingMapsKey}");


                Console.WriteLine();
                Console.WriteLine();

                // Configure Blob Output
                s_blobOutputManager = new BlobOutputManager(
                    storageAccountConnectionString,
                    webJobDashboardAndStorageConnectionString,
                    tmpContainer,
                    outputContainer,
                    maxBlobSize);
                await s_blobOutputManager.SetupAsync();

                // Configure EventHub Output
                var eventHubOutputManager = new EventHubOutputManager(outputEventHubConnectionString, outputEventHubName);

                ILocationLookupProvider lookupProvider;
                if (string.IsNullOrWhiteSpace(bingMapsKey))
                    lookupProvider = new NullLocationLookupProvider();
                else
                    lookupProvider = new BingLocationLookupProvider(bingMapsKey);

                // Setup Handler to use above configured output managers
                var handler = new GameEventHandler(s_blobOutputManager, eventHubOutputManager, lookupProvider);

                // Configure Router to switch handling to correct method depending on game event type
                s_router = new GameEventRouter(GameEventHandler.ResolveEventType,
                    GameEventHandler.UnknownGameEventFormatHandler,
                    GameEventHandler.UnknownGameEventTypeHandler,
                    handler.FlushAsync);

                s_router.RegisterKnownGameEventTypeHandler("count/v1.0.0", handler.HandleCountEventAsync);
                s_router.RegisterKnownGameEventTypeHandler("game-heartbeat/v1.0.0", handler.HandleGameHeartbeatAsync);
                s_router.RegisterKnownGameEventTypeHandler("game-start/v1.0.0", handler.HandleGameStartEventAsync);
                s_router.RegisterKnownGameEventTypeHandler("game-stop/v1.0.0", handler.HandleGameStopEventAsync);
                s_router.RegisterKnownGameEventTypeHandler("location/v1.0.0", handler.HandleLocationEventAsync);
                s_router.RegisterKnownGameEventTypeHandler("score/v1.0.0", handler.HandleScoreEventAsync);
                s_router.RegisterKnownGameEventTypeHandler("start/v1.0.0", handler.HandleStartEventAsync);
                s_router.RegisterKnownGameEventTypeHandler("stop/v1.0.0", handler.HandleStopEventAsync);
                s_router.RegisterKnownGameEventTypeHandler("generic/v1.0.0", handler.HandleGenericEventAsync);
                s_router.RegisterKnownGameEventTypeHandler("level-completed/v1.0.0", handler.HandleLevelCompletedEventAsync);
                s_router.RegisterKnownGameEventTypeHandler("level-start/v1.0.0", handler.HandleLevelStartEventAsync);
            }
        }

        public static async Task HandleBatchAsync([EventHubTrigger("%NETHER_INGEST_EVENTHUB_NAME%")] EventData[] events)
        {
            await InitializeGameEventReceiverAsync();
            var dequeueTime = DateTime.UtcNow;

            if (events.Length > 1)
                Console.WriteLine($"....Received batch of {events.Length} events");

            foreach (var ev in events)
            {
                var gameEventData = new GameEventData(ev, dequeueTime);
                await s_router.HandleGameEventAsync(gameEventData);
            }

            await s_router.FlushAsync();
        }

        /// <summary>
        /// Time triggered function - goes over all the blobs in the tmp container ones marked as copied
        /// </summary>
        public static async Task TimerJob([TimerTrigger("00:00:30")] TimerInfo timer)
        {
            Console.WriteLine("TimerJob triggered");
            await InitializeGameEventReceiverAsync();
            await s_blobOutputManager.AppendBlobCleanupAsync();
        }
    }
}