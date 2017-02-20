// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Azure.WebJobs.ServiceBus;
using Nether.Analytics.EventProcessor.Output.Blob;
using Nether.Analytics.EventProcessor.Output.EventHub;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace Nether.Analytics.EventProcessor
{
    /// <summary>
    /// Main class of the EventProcessor. This class has the required trigger(s) to
    /// get called by the WebJob SDK whenever there is a new Event to Process
    /// </summary>
    public static class GameEventReceiver
    {
        private static readonly GameEventRouter s_router;

        static GameEventReceiver()
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

            var maxBlobSize = 10 * 1024; // 10kB

            Console.WriteLine($"Max Blob Size: {maxBlobSize / 1024 / 1024}MB ({maxBlobSize}B)");
            Console.WriteLine();

            // Configure Blob Output
            var blobOutputManager = new BlobOutputManager(
                storageAccountConnectionString,
                tmpContainer,
                outputContainer,
                maxBlobSize);

            // Configure EventHub Output
            var eventHubOutputManager = new EventHubOutputManager(outputEventHubConnectionString, outputEventHubName);

            // Setup Handler to use above configured output managers
            var handler = new GameEventHandler(blobOutputManager, eventHubOutputManager);

            // Configure Router to switch handeling to correct method depending on game event type
            s_router = new GameEventRouter(GameEventHandler.ResolveEventType,
                GameEventHandler.UnknownGameEventFormatHandler,
                GameEventHandler.UnknownGameEventTypeHandler,
                handler.Flush);

            s_router.RegisterKnownGameEventTypeHandler("count/v1.0.0", handler.HandleCountEvent);
            s_router.RegisterKnownGameEventTypeHandler("game-heartbeat/v1.0.0", handler.HandleGameHeartbeat);
            s_router.RegisterKnownGameEventTypeHandler("game-start/v1.0.0", handler.HandleGameStartEvent);
            s_router.RegisterKnownGameEventTypeHandler("game-stop/v1.0.0", handler.HandleGameStopEvent);
            s_router.RegisterKnownGameEventTypeHandler("location/v1.0.0", handler.HandleLocationEvent);
            s_router.RegisterKnownGameEventTypeHandler("score/v1.0.0", handler.HandleScoreEvent);
            s_router.RegisterKnownGameEventTypeHandler("start/v1.0.0", handler.HandleStartEvent);
            s_router.RegisterKnownGameEventTypeHandler("stop/v1.0.0", handler.HandleStopEvent);
            s_router.RegisterKnownGameEventTypeHandler("generic/v1.0.0", handler.HandleGenericEvent);
            s_router.RegisterKnownGameEventTypeHandler("level-completed/v1.0.0", handler.HandleLevelCompletedEvent);
            s_router.RegisterKnownGameEventTypeHandler("level-start/v1.0.0", handler.HandleLevelStartEvent);
        }

        //public void HandleOne([EventHubTrigger("%NETHER_INGEST_EVENTHUB_NAME%")] string data)
        //{
        //    //TODO: Figure out how to configure above EventHubName now named ingest

        //    // Forward data to "router" in order to handle the event
        //    _router.HandleGameEvent(data);
        //}

        public static void HandleBatch([EventHubTrigger("%NETHER_INGEST_EVENTHUB_NAME%")] EventData[] events)
        {
            var dequeueTime = DateTime.UtcNow;

            if (events.Length > 1)
                Console.WriteLine($"....Received batch of {events.Length} events");

            foreach (var ev in events)
            {
                var gameEventData = new GameEventData(ev, dequeueTime);
                s_router.HandleGameEvent(gameEventData);
            }

            s_router.Flush();
        }
    }
}