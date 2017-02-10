// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Azure.WebJobs.ServiceBus;
using Nether.Analytics.EventProcessor.Output.Blob;
using Nether.Analytics.EventProcessor.Output.EventHub;
using System.Configuration;

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

            var outputStorageAccountConnectionString = ConfigResolver.Resolve("NETHER_ANALYTICS_STORAGE_CONNECTIONSTRING");
            Console.WriteLine($"outputStorageAccountConnectionString: {outputStorageAccountConnectionString}");

            var outputContainer = ConfigResolver.Resolve("NETHER_ANALYTICS_STORAGE_CONTAINER");
            Console.WriteLine($"outputContainer: {outputContainer}");

            var outputEventHubConnectionString = ConfigResolver.Resolve("NETHER_INTERMEDIATE_EVENTHUB_CONNECTIONSTRING");
            Console.WriteLine($"outputEventHubConnectionString: {outputEventHubConnectionString}");

            var outputEventHubName = ConfigResolver.Resolve("NETHER_INTERMEDIATE_EVENTHUB_NAME");
            Console.WriteLine($"outputEventHubName: {outputEventHubName}");

            var maxBlobSize = 100 * 1024 * 1024;

            Console.WriteLine($"Max Blob Size: {maxBlobSize / 1024 / 1024}MB ({maxBlobSize}B)");
            Console.WriteLine();

            // Configure Blob Output
            var blobOutputManager = new BlobOutputManager(
                outputStorageAccountConnectionString,
                outputContainer,
                BlobOutputFolderStructure.YearMonthDay,
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

            s_router.RegEventTypeAction("count", "1.0.0", handler.HandleCountEvent);
            s_router.RegEventTypeAction("game-heartbeat", "1.0.0", handler.HandleGameHeartbeat);
            s_router.RegEventTypeAction("game-start", "1.0.0", handler.HandleGameStartEvent);
            s_router.RegEventTypeAction("game-stop", "1.0.0", handler.HandleGameStopEvent);
            s_router.RegEventTypeAction("location", "1.0.0", handler.HandleLocationEvent);
            s_router.RegEventTypeAction("score", "1.0.0", handler.HandleScoreEvent);
            s_router.RegEventTypeAction("start", "1.0.0", handler.HandleStartEvent);
            s_router.RegEventTypeAction("stop", "1.0.0", handler.HandleStopEvent);
            s_router.RegEventTypeAction("generic", "1.0.0", handler.HandleGenericEvent);
        }

        //public void HandleOne([EventHubTrigger("%NETHER_INGEST_EVENTHUB_NAME%")] string data)
        //{
        //    //TODO: Figure out how to configure above EventHubName now named ingest

        //    // Forward data to "router" in order to handle the event
        //    _router.HandleGameEvent(data);
        //}

        public static void HandleBatch([EventHubTrigger("%NETHER_INGEST_EVENTHUB_NAME%")] string[] events)
        {
            if (events.Length > 1)
                Console.WriteLine($"....Received batch of {events.Length} events");
            foreach (var e in events)
            {
                s_router.HandleGameEvent(e);
            }

            s_router.Flush();
        }
    }
}