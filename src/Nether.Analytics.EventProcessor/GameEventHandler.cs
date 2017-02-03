using System.Text;
using Newtonsoft.Json.Linq;

namespace Nether.Analytics.EventProcessor
{
    public class GameEventHandler
    {
        private readonly BlobAppender _blobAppender;
        private readonly EventHubWriter _eventHubWriter;

        public GameEventHandler(BlobAppender blobAppender, EventHubWriter eventHubWriter)
        {
            _blobAppender = blobAppender;
            _eventHubWriter = eventHubWriter;
        }

        public void HandleGameStartEvent(string gameEventType, string jsonEvent)
        {
            var csvEvent = jsonEvent.JsonToCsvString("type", "version", "clientUtcTime", "gameSessionId", "gamerTag");

            _blobAppender.WriteToBlob(gameEventType, csvEvent);
        }

        public void HandleGameHeartbeat(string gameEventType, string jsonEvent)
        {
            var csvEvent = jsonEvent.JsonToCsvString("type", "version", "clientUtcTime", "gameSessionId");

            _blobAppender.WriteToBlob(gameEventType, csvEvent);
            _eventHubWriter.SendToEventHub(gameEventType, csvEvent);
        }

        public static string ResolveEventType(string gameEvent)
        {
            var json = JObject.Parse(gameEvent);
            var gameEventType = (string)json["type"];
            var version = (string)json["version"];

            return VersionedName(gameEventType, version);
        }

        public static string VersionedName(string gameEventType, string version)
        {
            return $"{gameEventType}|{version}";
        }


    }
}