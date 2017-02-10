// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Nether.Analytics.EventProcessor.Output.Blob;
using Nether.Analytics.EventProcessor.Output.EventHub;
using Newtonsoft.Json.Linq;

namespace Nether.Analytics.EventProcessor
{
    /// <summary>
    ///     Class that contains default Game Event Handling in Nether. The correct
    ///     methods will be called by the GameEventRouter depending on GameEventType.
    ///     Extend, override or replace this class and the contained actions in order
    ///     to implement different game event handling in Nether.
    /// </summary>
    public class GameEventHandler
    {
        private readonly BlobOutputManager _blobOutputManager;
        private readonly EventHubOutputManager _eventHubOutputManager;

        public GameEventHandler(BlobOutputManager blobOutputManager, EventHubOutputManager eventHubOutputManager)
        {
            _blobOutputManager = blobOutputManager;
            _eventHubOutputManager = eventHubOutputManager;
        }

        public void Flush()
        {
            _blobOutputManager.FlushWriteQueues();
        }

        //TODO: Fix Game Event Handlers to use reflection over properties if possible
        //TODO: Enrich events with knows facts, such as location on heartbeats, etc.
        public void HandleCountEvent(string gameEventType, string jsonEvent)
        {
            var csvEvent = jsonEvent.JsonToCsvString("type", "version", "clientUtcTime", "displayName", "value",
                "gameSessionId");

            _blobOutputManager.QueueAppendToBlob(gameEventType, csvEvent);
            _eventHubOutputManager.SendToEventHub(gameEventType, csvEvent);
        }

        public void HandleGameHeartbeat(string gameEventType, string jsonEvent)
        {
            var csvEvent = jsonEvent.JsonToCsvString("type", "version", "clientUtcTime", "gameSessionId");

            _blobOutputManager.QueueAppendToBlob(gameEventType, csvEvent);
            _eventHubOutputManager.SendToEventHub(gameEventType, csvEvent);
        }

        public void HandleGameStartEvent(string gameEventType, string jsonEvent)
        {
            var csvEvent = jsonEvent.JsonToCsvString("type", "version", "clientUtcTime", "gameSessionId", "gamerTag");

            _blobOutputManager.QueueAppendToBlob(gameEventType, csvEvent);
        }

        public void HandleGameStopEvent(string gameEventType, string jsonEvent)
        {
            var csvEvent = jsonEvent.JsonToCsvString("type", "version", "clientUtcTime", "gameSessionId");

            _blobOutputManager.QueueAppendToBlob(gameEventType, csvEvent);
        }

        public void HandleGenericEvent(string gameEventType, string jsonEvent)
        {
            var csvEvent = jsonEvent.JsonToCsvString("type", "version");

            _blobOutputManager.QueueAppendToBlob(gameEventType, csvEvent);
        }

        public void HandleLocationEvent(string gameEventType, string jsonEvent)
        {
            //TODO: Implement more properties on Location Event
            var csvEvent = jsonEvent.JsonToCsvString("type", "version", "clientUtcTime", "gameSessionId");

            _blobOutputManager.QueueAppendToBlob(gameEventType, csvEvent);
        }

        public void HandleScoreEvent(string gameEventType, string jsonEvent)
        {
            var csvEvent = jsonEvent.JsonToCsvString("type", "version", "clientUtcTime", "gameSessionId", "score");

            _blobOutputManager.QueueAppendToBlob(gameEventType, csvEvent);
        }

        public void HandleStartEvent(string gameEventType, string jsonEvent)
        {
            var csvEvent = jsonEvent.JsonToCsvString("type", "version", "clientUtcTime", "eventCorrelationId",
                "displayName", "gameSessionId");

            _blobOutputManager.QueueAppendToBlob(gameEventType, csvEvent);
        }

        public void HandleStopEvent(string gameEventType, string jsonEvent)
        {
            var csvEvent = jsonEvent.JsonToCsvString("type", "version", "clientUtcTime", "eventCorrelationId",
                "gameSessionId");

            _blobOutputManager.QueueAppendToBlob(gameEventType, csvEvent);
        }

        /// <summary>
        ///     Inspects gameEvent to figure out what GameEventType we are working with.
        ///     This implementation assumes messages as sent as JSON with two properties
        ///     "type" and "version".
        /// </summary>
        /// <param name="gameEvent">The JSON Game Event to inspect</param>
        /// <returns>The Game Event Type</returns>
        public static string ResolveEventType(string gameEvent)
        {
            var json = JObject.Parse(gameEvent);
            var gameEventType = (string)json["type"];
            var version = (string)json["version"];

            if (gameEventType == null || version == null)
                throw new ApplicationException(
                    "Unable to resolve Game Event Type, since game event doesn't contain type and/or version property");

            return VersionedName(gameEventType, version);
        }


        public static void UnknownGameEventFormatHandler(string data)
        {
            Console.WriteLine("Unknown Game Event Format found in ...");
            Console.WriteLine(data);
        }

        public static void UnknownGameEventTypeHandler(string gameEventType, string data)
        {
            Console.WriteLine("Unknown and unhandled Game Event Type found in ...");
            Console.WriteLine(data);
        }

        /// <summary>
        ///     Combines Event Type and Version to a "versioned name"
        /// </summary>
        /// <param name="gameEventType">Game Event Type</param>
        /// <param name="version">Version of Game Event Type</param>
        /// <returns>A combination of version and name</returns>
        public static string VersionedName(string gameEventType, string version)
        {
            return $"{gameEventType}/v{version}";
        }
    }
}