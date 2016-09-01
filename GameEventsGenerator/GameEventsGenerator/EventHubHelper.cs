using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus;
using RetryPolicy = Microsoft.Practices.TransientFaultHandling.RetryPolicy;
using Microsoft.Practices.TransientFaultHandling;

namespace GameEventsGenerator
{
    class EventHubHelper
    {
        // TODO: what is it for?
        private static readonly RetryPolicy RetryPolicy = new RetryPolicy(
            new EventHubTransientErrorDetectionStrategy(),
            new ExponentialBackoff(
                "EventHubInputAdapter",
                5,
                TimeSpan.FromMilliseconds(100),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(500),
                true));


        /// <summary>
        /// Creates an event hub in given service bus namespace
        /// </summary>
        /// <param name="eventHubConnectionString">Connection string to access the service bus namespace</param>
        /// <param name="eventHubName">Name of event hub to be created</param>
        /// <param name="partitionCount">Number of partitions in event hub</param>
        public static void CreateEventHubIfNotExists(string eventHubConnectionString, string eventHubName, int partitionCount = 0)
        {
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(eventHubConnectionString);
            var eventHubDescription = new EventHubDescription(eventHubName);
            if (partitionCount > 0)
            {
                eventHubDescription.PartitionCount = partitionCount;
            }

            RetryPolicy.ExecuteAsync(() => namespaceManager.CreateEventHubIfNotExistsAsync(eventHubDescription)).Wait();
        }

        public static void DeleteAllEventHubs(string eventHubConnectionString)
        {
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(eventHubConnectionString);
            Console.WriteLine("Deleting the EventHub data in event hub with connection string: '{0}'", eventHubConnectionString);

            foreach (var eventhub in RetryPolicy.ExecuteAsync(namespaceManager.GetEventHubsAsync).Result)
            {
                Console.WriteLine("Deleting Event Hub '{0}'", eventhub.Path);
                RetryPolicy.ExecuteAsync(() => namespaceManager.DeleteEventHubAsync(eventhub.Path)).Wait();
            }
        }

        private class EventHubTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
        {
            public bool IsTransient(Exception ex)
            {
                var messagingException = ex as MessagingException;
                if ((messagingException != null && messagingException.IsTransient) || ex is TimeoutException)
                {
                    Console.WriteLine(ex);
                    return true;
                }
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
