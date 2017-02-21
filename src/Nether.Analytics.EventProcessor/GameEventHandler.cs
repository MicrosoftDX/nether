// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Nether.Analytics.EventProcessor.Output.Blob;
using Nether.Analytics.EventProcessor.Output.EventHub;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using NGeoHash.Portable;
using Nether.Analytics.EventProcessor.GameEvents;

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
        public void HandleCountEvent(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("displayName", "value", "gameSessionId");

            _blobOutputManager.QueueAppendToBlob(data, serializedGameEvent);
            _eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public void HandleGameHeartbeat(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId");

            _blobOutputManager.QueueAppendToBlob(data, serializedGameEvent);
            _eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public void HandleGameStartEvent(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId", "gamerTag");

            _blobOutputManager.QueueAppendToBlob(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public void HandleGameStopEvent(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId");

            _blobOutputManager.QueueAppendToBlob(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public void HandleGenericEvent(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv();

            _blobOutputManager.QueueAppendToBlob(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public void HandleLocationEvent(GameEventData data)
        {
            const int GeoHashBitPrecision = 32; //bits

            var inEvent = JsonConvert.DeserializeObject<IncommingLocationEvent>(data.Data);

            var geoHash = GeoHash.EncodeInt(inEvent.Lat, inEvent.Lon, GeoHashBitPrecision);
            var geoHashCenterCoordinates = GeoHash.DecodeInt(geoHash, GeoHashBitPrecision).Coordinates;

            var outEvent = new OutgoingLocationEvent
            {
                EnqueTime = data.EnqueuedTime,
                DequeTime = data.DequeuedTime,
                ClientUtcTime = inEvent.ClientUtcTime,
                GameSessionId = inEvent.GameSessionId,
                Lat = inEvent.Lat,
                Lon = inEvent.Lon,
                GeoHash = geoHash,
                GeoHashPrecision = GeoHashBitPrecision,
                GeoHashCenterLat = geoHashCenterCoordinates.Lat,
                GeoHashCenterLon = geoHashCenterCoordinates.Lon,
                Properties = inEvent.Properties
            };

            //TODO: Optimize this so we don't serialize back to JSON first and then to CSV

            var jsonObject = JsonConvert.SerializeObject(
                outEvent,
                Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            data.Data = jsonObject;

            var csvObject = data.ToCsv("gameSessionId", "lat", "lon",
                "geoHash", "geoHashPrecision",
                "geoHashCenterLat", "geoHashCenterLon");

            // Output CSV to BLOB Storage and JSON to StreamAnalytics (via EventHub)
            _blobOutputManager.QueueAppendToBlob(data, csvObject);
            _eventHubOutputManager.SendToEventHub(data, jsonObject); 
        }

        public void HandleScoreEvent(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId", "score");

            _blobOutputManager.QueueAppendToBlob(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public void HandleStartEvent(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("eventCorrelationId", "displayName", "gameSessionId");

            _blobOutputManager.QueueAppendToBlob(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public void HandleStopEvent(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("eventCorrelationId");

            _blobOutputManager.QueueAppendToBlob(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public void HandleLevelCompletedEvent(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId", "level");

            _blobOutputManager.QueueAppendToBlob(data, serializedGameEvent);
        }

        public void HandleLevelStartEvent(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId", "level");

            _blobOutputManager.QueueAppendToBlob(data, serializedGameEvent);
        }

        /// <summary>
        ///     Inspects gameEvent to figure out what GameEventType we are working with.
        ///     This implementation assumes messages as sent as JSON with two properties
        ///     "type" and "version".
        /// </summary>
        /// <param name="gameEvent">The JSON Game Event to inspect</param>
        /// <returns>The Game Event Type</returns>
        public static void ResolveEventType(GameEventData gameEvent)
        {
            var json = JObject.Parse(gameEvent.Data);
            var gameEventType = (string)json["type"];
            var version = (string)json["version"];

            if (gameEventType == null || version == null)
                throw new ApplicationException(
                    "Unable to resolve Game Event Type, since game event doesn't contain type and/or version property");

            gameEvent.EventType = gameEventType;
            gameEvent.EventVersion = version;
        }


        public static void UnknownGameEventFormatHandler(GameEventData gameEvent)
        {
            Console.WriteLine("Unknown Game Event Format found in ...");
            Console.WriteLine(gameEvent.Data);
        }

        public static void UnknownGameEventTypeHandler(GameEventData gameEvent)
        {
            Console.WriteLine($"Unknown and unhandled Game Event Type [{gameEvent.VersionedType}] found in event ...");
            Console.WriteLine(gameEvent.Data);
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