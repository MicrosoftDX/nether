using System;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Nether.Analytics.EventProcessor
{
    /// <summary>
    /// Helper methods
    /// </summary>
    public static class EventProcessorExtensions
    {

        private const string CsvDelimiter = "|";

        public static void RegEventTypeAction(this GameEventRouter router, string gameEventType, string version, Action<string, string> action)
        {
            router.RegisterKnownGameEventTypeHandler(GameEventHandler.VersionedName(gameEventType, version), action);
        }

        public static string JsonToCsvString(this string jsonString, params string[] properties)
        {
            var json = JObject.Parse(jsonString);
            var builder = new StringBuilder();

            foreach (var property in properties)
            {
                if (builder.Length > 0)
                    builder.Append(CsvDelimiter);

                builder.Append(json[property]);
            }

            return builder.ToString();
        }
    }
}