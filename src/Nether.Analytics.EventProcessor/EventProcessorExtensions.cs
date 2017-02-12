// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using System;
//using System.Collections.Generic;
//using System.Text;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace Nether.Analytics.EventProcessor
//{
//    /// <summary>
//    /// Helper methods
//    /// </summary>
//    public static class EventProcessorExtensions
//    {
//        private const string CsvDelimiter = "|";
//        private const string PropDelimiter = ";";

//        public static void RegEventTypeAction(this GameEventRouter router, string gameEventType, string version, Action<string, string> action)
//        {
//            router.RegisterKnownGameEventTypeHandler(GameEventHandler.VersionedName(gameEventType, version), action);
//        }

//        //TODO Move this method away from being an extension method since it's not generic enough any more
//        public static string JsonToCsvString(this string jsonString, params string[] props)
//        {
//            var json = JObject.Parse(jsonString);
//            var builder = new StringBuilder();

//            foreach (var prop in props)
//            {
//                builder.Append(json[prop]);
//                builder.Append(CsvDelimiter);
//            }

//            var properties = json["properties"];
//            if (properties != null)
//            {
//                var propDict = properties.ToObject<Dictionary<string, string>>();
//                var propBuilder = new StringBuilder();

//                foreach (var key in propDict.Keys)
//                {
//                    if (propBuilder.Length > 0)
//                        propBuilder.Append(PropDelimiter);
//                    propBuilder.Append($"{key}={propDict[key]}");
//                }

//                builder.Append(propBuilder);
//            }

//            return builder.ToString();
//        }
//    }
//}