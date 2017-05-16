// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Analytics.EventHubs;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics.Parsers
{
    public class EventHubListenerMessageJsonParser : IMessageParser<EventHubListenerMessage>
    {
        public EventHubListenerMessageJsonParser(ICorruptMessageHandler corruptMessageHandler)
        {
            _corruptMessageHandler = corruptMessageHandler;
        }

        private ICorruptMessageHandler _corruptMessageHandler;
        public ICorruptMessageHandler CorruptMessageHandler
        {
            get { return _corruptMessageHandler; }
        }

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
                await _corruptMessageHandler.HandleAsync(data);
                return null;
            }

            var gameEventType = (string)json["type"];
            var version = (string)json["version"];

            if (gameEventType == null || version == null)
            {
                await _corruptMessageHandler.HandleAsync(data);
                return null;
            }

            var id = unparsedMsg.PartitionId + "_" + unparsedMsg.SequenceNumber;

            var msg = new Message
            {
                Id = id,
                MessageType = gameEventType,
                Version = version,
                EnqueueTimeUtc = unparsedMsg.EnqueuedTime
            };

            msg.Properties["id"] = id;
            msg.Properties["enqueueTimeUtc"] = msg.EnqueueTimeUtc.ToString();

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
                    Console.WriteLine($"Unable to parse property:{key} as string on {msg.MessageType}");
                    Console.WriteLine("WARNING: Continuing anyway!!! TODO: Fix this parsing problem!!!");
                }
            }

            return msg;
        }
    }
}
