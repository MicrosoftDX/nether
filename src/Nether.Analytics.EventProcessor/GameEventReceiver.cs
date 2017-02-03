using Microsoft.Azure.WebJobs.ServiceBus;

namespace Nether.Analytics.EventProcessor
{
    public class GameEventReceiver
    {
        private readonly GameEventRouter _router;
        private readonly GameEventHandler _handler;
        private readonly BlobAppender _blobAppender;
        private readonly EventHubWriter _eventHubWriter;

        public GameEventReceiver()
        {
            //TODO: Fix Configuration of BlobAppender and EventHubWriter
            _blobAppender = new BlobAppender();
            _eventHubWriter = new EventHubWriter();

            _handler = new GameEventHandler(_blobAppender, _eventHubWriter);

            // Configure Router to switch handeling to correct method depending on game event type
            _router = new GameEventRouter(GameEventHandler.ResolveEventType);
            _router.RegHandler("game-start", "1.0.0", _handler.HandleGameStartEvent);
            _router.RegHandler("game-heartbeat", "1.0.0", _handler.HandleGameHeartbeat);
        }

        public void HandleOne([EventHubTrigger("analytics")] string data)
        {
            _router.HandleGameEvent(data);
        }
    }
}