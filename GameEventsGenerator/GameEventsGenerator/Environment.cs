using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace GameEventsGenerator
{
    internal class Environment
    {
        internal static string EntryEventHubPath = ConfigurationManager.AppSettings["EntryEventHub"];
        internal static string ExitEventHubPath = ConfigurationManager.AppSettings["ExitEventHub"];
        internal static string EventHubConnectionString = ConfigurationManager.AppSettings["EventHubConnectionString"];
        internal static string EventHubPath = ConfigurationManager.AppSettings["EventHub"];

        public static void SetupEventHubs()
        {
            // Entry Event Hub
            EventHubHelper.CreateEventHubIfNotExists(EventHubConnectionString, EntryEventHubPath);
            // Exit Event Hub
            EventHubHelper.CreateEventHubIfNotExists(EventHubConnectionString, ExitEventHubPath);
            // Single stream event hub
            EventHubHelper.CreateEventHubIfNotExists(EventHubConnectionString, EventHubPath);
        }

        public static void Cleanup()
        {
            EventHubHelper.DeleteAllEventHubs(EventHubConnectionString);
        }

        internal static void Setup()
        {
            SetupEventHubs();
        }
    }
}
