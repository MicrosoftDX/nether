using Microsoft.ServiceBus.Messaging;

namespace Nether.Analytics.EventProcessor.Output.EventHub
{
    public class EventHubOutputManager
    {
        private readonly string _eventHubConnectionString;

        public EventHubOutputManager(string eventHubConnectionString)
        {
            _eventHubConnectionString = eventHubConnectionString;
        }

        public void SendTo(string gameEventType, string data)
        {
            var client = EventHubClient.CreateFromConnectionString(_eventHubConnectionString);
            //TODO: Write to EventHub here!!!

        }
    }
}