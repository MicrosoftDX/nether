// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Azure.WebJobs.ServiceBus;
using Nether.Analytics.EventProcessor.Output.Blob;
using Nether.Analytics.EventProcessor.Output.EventHub;

namespace Nether.Analytics.EventProcessor
{
    /// <summary>
    /// Main class of the EventProcessor. This class has the required trigger(s) to
    /// get called by the WebJob SDK whenever there is a new Event to Process
    /// </summary>
    public class GameEventReceiver
    {
        private readonly GameEventRouter _router;
        private readonly GameEventHandler _handler;
        private readonly BlobOutputManager _blobOutputManager;
        private readonly EventHubOutputManager _eventHubOutputManager;

        public GameEventReceiver()
        {
            // Read Environment Variables for configuration
            // TODO: Fix configuration to be in line with other configuration in Nether

            Console.WriteLine("Configuring GameEventReceiver (from Environment Variables");
            string outputStorageAccountConnectionString =
                Environment.GetEnvironmentVariable("NETHER_ANALYTICS_STORAGE_CONNECTIONSTRING");
            Console.WriteLine($"outputStorageAccountConnectionString: {outputStorageAccountConnectionString}");
            string outputContainer =
                Environment.GetEnvironmentVariable("NETHER_ANALYTICS_STORAGE_CONTAINER");
            Console.WriteLine($"outputContainer: {outputContainer}");
            string outputEventHubConnectionString =
                Environment.GetEnvironmentVariable("NETHER_INTERMEDIATE_EVENTHUB_CONNECTIONSTRING");
            Console.WriteLine($"outputEventHubConnectionString: {outputEventHubConnectionString}");
            string outputEventHubName =
                Environment.GetEnvironmentVariable("NETHER_INTERMEDIATE_EVENTHUB_NAME");
            Console.WriteLine($"outputEventHubName: {outputEventHubName}");
            Console.WriteLine();

            // Configure Blob Output
            _blobOutputManager = new BlobOutputManager(
                outputStorageAccountConnectionString,
                outputContainer,
                BlobOutputFolderStructure.YearMonthDayHour,
                100 * 1024 * 1024); // 100MB

            // Configure EventHub Output
            _eventHubOutputManager = new EventHubOutputManager(outputEventHubConnectionString, outputEventHubName);

            // Setup Handler to use above configured output managers
            _handler = new GameEventHandler(_blobOutputManager, _eventHubOutputManager);

            // Configure Router to switch handeling to correct method depending on game event type
            _router = new GameEventRouter(GameEventHandler.ResolveEventType,
                GameEventHandler.UnknownGameEventFormatHandler,
                GameEventHandler.UnknownGameEventTypeHandler);

            _router.RegEventTypeAction("count", "1.0.0", _handler.HandleCountEvent);
            _router.RegEventTypeAction("game-heartbeat", "1.0.0", _handler.HandleGameHeartbeat);
            _router.RegEventTypeAction("game-start", "1.0.0", _handler.HandleGameStartEvent);
            _router.RegEventTypeAction("game-stop", "1.0.0", _handler.HandleGameStopEvent);
            _router.RegEventTypeAction("location", "1.0.0", _handler.HandleLocationEvent);
            _router.RegEventTypeAction("score", "1.0.0", _handler.HandleScoreEvent);
            _router.RegEventTypeAction("start", "1.0.0", _handler.HandleStartEvent);
            _router.RegEventTypeAction("stop", "1.0.0", _handler.HandleStopEvent);
        }

        /// <summary>
        /// Method gets called automatically by the WebJobs SDK whenever there is a new
        /// event on the monitored EventHub. Acts as the starting point for any Game Event
        /// that gets processed by the Event Processor.
        /// </summary>
        /// <param name="data">The raw Game Event Data to be processed</param>
        public void HandleOne([EventHubTrigger("ingest")] string data)
        {
            //TODO: Figure out how to configure above EventHubName now named ingest

            // Forward data to "router" in order to handle the event
            _router.HandleGameEvent(data);
        }
    }
}