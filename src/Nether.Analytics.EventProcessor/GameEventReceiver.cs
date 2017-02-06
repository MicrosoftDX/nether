using Microsoft.Azure.WebJobs.ServiceBus;
using Nether.Analytics.EventProcessor.Output.Blob;
using Nether.Analytics.EventProcessor.Output.EventHub;

namespace Nether.Analytics.EventProcessor
{
    /// <summary>
    /// Main class of the EventProcessor. This class has the required trigger(s) to
    /// get called by the WebJob SDK whenever there is a new Event to Process
    /// </summary>
    public class GameEventReceiver
    {
        private readonly GameEventRouter _router;
        private readonly GameEventHandler _handler;
        private readonly BlobOutputManager _blobOutputManager;
        private readonly EventHubOutputManager _eventHubOutputManager;

        private string _outputStorageAccountConnectionString;
        private string _outputEventHubConnectionString;

        public GameEventReceiver()
        {
            //TODO: Fix Configuration of BlobOutputManager and EventHubOutputManager

            // Configure Blob Output
            _blobOutputManager = new BlobOutputManager(
                _outputStorageAccountConnectionString,
                "gameevents",
                BlobOutputFolderStructure.YearMonthDayHour,
                100 * 1024 * 1024); // 100MB

            // Configure EventHub Output
            _eventHubOutputManager = new EventHubOutputManager(_outputEventHubConnectionString);

            // Setup Handler to use above configured output managers
            _handler = new GameEventHandler(_blobOutputManager, _eventHubOutputManager);

            // Configure Router to switch handeling to correct method depending on game event type
            _router = new GameEventRouter(GameEventHandler.ResolveEventType,
                GameEventHandler.UnknownGameEventFormatHandler,
                GameEventHandler.UnknownGameEventTypeHandler);

            _router.RegEventTypeAction("game-start", "1.0.0", _handler.HandleGameStartEvent);
            _router.RegEventTypeAction("game-heartbeat", "1.0.0", _handler.HandleGameHeartbeat);
        }

        /// <summary>
        /// Method gets called automatically by the WebJobs SDK whenever there is a new
        /// event on the monitored EventHub. Acts as the starting point for any Game Event
        /// that gets processed by the Event Processor.
        /// </summary>
        /// <param name="data">The raw Game Event Data to be processed</param>
        public void HandleOne([EventHubTrigger("incomming")] string data)
        {
            // Forward data to "router" in order to handle the event
            _router.HandleGameEvent(data);
        }
    }
}