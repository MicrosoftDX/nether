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
using Nether.Analytics.EventProcessor.EventTypeHandlers;
using System.Threading.Tasks;

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
        private readonly ILocationLookupProvider _locationLookupProvider;

        public GameEventHandler(BlobOutputManager blobOutputManager, EventHubOutputManager eventHubOutputManager, ILocationLookupProvider locationLookupProvider)
        {
            _blobOutputManager = blobOutputManager;
            _eventHubOutputManager = eventHubOutputManager;
            _locationLookupProvider = locationLookupProvider;
        }

        public async Task FlushAsync()
        {
            await _blobOutputManager.FlushWriteQueuesAsync();
        }

        //TODO: Fix Game Event Handlers to use reflection over properties if possible
        //TODO: Enrich events with knows facts, such as location on heartbeats, etc.
        public async Task HandleCountEventAsync(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("displayName", "value", "gameSessionId");

            await _blobOutputManager.QueueAppendToBlobAsync(data, serializedGameEvent);
            await _eventHubOutputManager.SendToEventHubAsync(data, serializedGameEvent);
        }

        public async Task HandleGameHeartbeatAsync(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId");

            await _blobOutputManager.QueueAppendToBlobAsync(data, serializedGameEvent);
            await _eventHubOutputManager.SendToEventHubAsync(data, serializedGameEvent);
        }

        public async Task HandleGameStartEventAsync(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId", "gamerTag");

            await _blobOutputManager.QueueAppendToBlobAsync(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public async Task HandleGameStopEventAsync(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId");

            await _blobOutputManager.QueueAppendToBlobAsync(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public async Task HandleGenericEventAsync(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv();

            await _blobOutputManager.QueueAppendToBlobAsync(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public async Task HandleLocationEventAsync(GameEventData data)
        {
            const int GeoHashBitPrecision = 32; //bits
            const int LocationLookupGeoHashBitPrecistion = 30; //bits

            var inEvent = JsonConvert.DeserializeObject<IncommingLocationEvent>(data.Data);

            var geoHash = GeoHash.EncodeInt(inEvent.Lat, inEvent.Lon, GeoHashBitPrecision);
            var geoHashCenterCoordinates = GeoHash.DecodeInt(geoHash, GeoHashBitPrecision).Coordinates;
            var locationLookupGeoHash = GeoHash.EncodeInt(inEvent.Lat, inEvent.Lon, LocationLookupGeoHashBitPrecistion);

            var l = new LocationEventHandler(_locationLookupProvider);
            var location = l.LookupGeoHash(locationLookupGeoHash, LocationLookupGeoHashBitPrecistion);

            var outEvent = new OutgoingLocationEvent
            {
                EnqueueTime = data.EnqueuedTime,
                DequeueTime = data.DequeuedTime,
                ClientUtcTime = inEvent.ClientUtcTime,
                GameSessionId = inEvent.GameSessionId,
                Lat = inEvent.Lat,
                Lon = inEvent.Lon,
                GeoHash = geoHash,
                GeoHashPrecision = GeoHashBitPrecision,
                GeoHashCenterLat = geoHashCenterCoordinates.Lat,
                GeoHashCenterLon = geoHashCenterCoordinates.Lon,
                Country = location.Country,
                District = location.District,
                City = location.City,
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
                "geoHashCenterLat", "geoHashCenterLon", "country", "district", "city");

            // Output CSV to BLOB Storage and JSON to StreamAnalytics (via EventHub)
            await _blobOutputManager.QueueAppendToBlobAsync(data, csvObject);
            await _eventHubOutputManager.SendToEventHubAsync(data, jsonObject);
        }



        public async Task HandleScoreEventAsync(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId", "score");

            await _blobOutputManager.QueueAppendToBlobAsync(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public async Task HandleStartEventAsync(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("eventCorrelationId", "displayName", "gameSessionId");

            await _blobOutputManager.QueueAppendToBlobAsync(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public async Task HandleStopEventAsync(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("eventCorrelationId");

            await _blobOutputManager.QueueAppendToBlobAsync(data, serializedGameEvent);
            //_eventHubOutputManager.SendToEventHub(data, serializedGameEvent);
        }

        public async Task HandleLevelCompletedEventAsync(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId", "level");

            await _blobOutputManager.QueueAppendToBlobAsync(data, serializedGameEvent);
        }

        public async Task HandleLevelStartEventAsync(GameEventData data)
        {
            var serializedGameEvent = data.ToCsv("gameSessionId", "level");

            await _blobOutputManager.QueueAppendToBlobAsync(data, serializedGameEvent);
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