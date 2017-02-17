// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.ServiceBus.Messaging;
using System.Text;
using System.Threading;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Nether.Analytics.EventProcessor
{
    public class GameEventData
    {
        private DateTime _enqueuedTime;
        private DateTime _dequeuedTime;

        public GameEventData(EventData eventData, DateTime dequeuedTime)
        {
            Data = Encoding.UTF8.GetString(eventData.GetBytes());
            _enqueuedTime = eventData.EnqueuedTimeUtc;
            _dequeuedTime = dequeuedTime;
        }

        public string Data { get; set; }
        public DateTime EnqueuedTime => _enqueuedTime;
        public DateTime DequeuedTime => _dequeuedTime;
        public string EventType { get; set; } = "Unknown";
        public string EventVersion { get; set; } = "Unknown";
        public string VersionedType => $"{EventType}/v{EventVersion}";

        public string ToCsv(params string[] props)
        {
            var line = new StringBuilder();

            // Always include EventType, EventVersion, EnqueuedTime, DequeuedTime, ClientTime in output
            // Note: All times are in UTC

            line.AppendColumns(EventType, EventVersion, EnqueuedTime, DequeuedTime);

            // Asume that EventData is provided as JSON
            var json = JObject.Parse(Data);

            // Every known event type also includes ClientUtcTime
            var clientUtcTime = DateTime.Parse(json["clientUtcTime"].ToString());
            line.AppendColumns(clientUtcTime);

            // Add all other properties that should be included in output
            foreach (var prop in props)
            {
                line.AppendColumns(json[prop]);
            }

            // All IGameEvents includes a properties-tag that should be automatically included in output as the last column
            var properties = json["properties"];
            if (properties != null && properties.HasValues)
            {
                var propDict = properties.ToObject<Dictionary<string, string>>();
                var propertiesColumn = new StringBuilder();

                foreach (var key in propDict.Keys)
                {
                    propertiesColumn.AppendProperties($"{key}={propDict[key]}");
                }

                line.AppendColumns(propertiesColumn);
            }
            else
            {
                // Append empty column to keep line format intact with same number of columns even if properties doesn't exist
                line.AppendColumns("");
            }

            return line.ToString();
        }
    }
}