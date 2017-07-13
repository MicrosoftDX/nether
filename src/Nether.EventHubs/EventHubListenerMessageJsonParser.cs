// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Ingest;
using Nether.Ingest.Parsers;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Nether.EventHubs
{
    public class EventHubListenerMessageJsonParser : IMessageParser<EventHubListenerMessage>
    {
        public Func<string, Task> CorruptMessageAsyncFunc { get; set; } = null;
        public bool AllowDbgEnqueuedTime { get; set; } = false;
        public string MessageTypePropertyName { get; set; } = "type";
        public string MessageVersionPropertyName { get; set; } = "version";
        public bool UseStaticMessageVersion { get; set; } = false;
        public string StaticMessageVersion { get; set; } = null;

        public async Task<Message> ParseMessageAsync(EventHubListenerMessage unparsedMsg)
        {
            var data = Encoding.UTF8.GetString(unparsedMsg.Body.Array, unparsedMsg.Body.Offset, unparsedMsg.Body.Count);

            JObject json;
            try
            {
                json = JObject.Parse(data);
            }
            catch //JSON serialization failed
            {
                await CorruptMessageAsyncFunc?.Invoke(data);
                return null;
            }

            // Game Event Type
            var gameEventType = (string)json[MessageTypePropertyName];
            if (gameEventType == null)
            {
                await CorruptMessageAsyncFunc?.Invoke(data);
                return null;
            }

            // Game Event Version
            string version;

            if (UseStaticMessageVersion && !string.IsNullOrWhiteSpace(StaticMessageVersion))
            {
                version = StaticMessageVersion;
            }
            else
            {
                try
                {
                    version = (string)json["version"];
                }
                catch
                {
                    await CorruptMessageAsyncFunc?.Invoke(data);
                    return null;
                }
            }

            var dbgEnqueuedTime = (string)json["dbgEnqueuedTimeUtc"];


            // If dbgEnqueuedTime is allowed and that property is set, then use it instead of the DateTime given by EventHub
            var enqueuedTime = (AllowDbgEnqueuedTime && dbgEnqueuedTime != null) ? DateTime.Parse(dbgEnqueuedTime) : unparsedMsg.EnqueuedTime;

            var id = unparsedMsg.PartitionId + "_" + unparsedMsg.SequenceNumber;

            var msg = new Message
            {
                Id = id,
                Type = gameEventType,
                Version = version,
                EnqueuedTimeUtc = enqueuedTime,
            };

            msg.Properties["id"] = id;
            msg.Properties["enqueuedTimeUtc"] = msg.EnqueuedTimeUtc.ToString();

            foreach (var p in json)
            {
                //TODO: Replace below naive JSON parsing implementation with more sophisticated and robust

                var key = p.Key;
                try
                {
                    var value = (string)json[p.Key];

                    msg.Properties[key] = value;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Unable to parse property:{key} as string on {msg.Type}");
                    Console.WriteLine("WARNING: Continuing anyway!!! TODO: Fix this parsing problem!!!");
                }
            }

            return msg;
        }
    }
}
