using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using System.Globalization;
using System.Configuration;

namespace GameEventsGenerator
{
    class Program
    {
        private static Timer timer;
        static void Main(string[] args)
        {
            //SendData(Environment.EventHubConnectionString, Environment.EntryEventHubPath, Environment.ExitEventHubPath);
            SendData(Environment.EventHubConnectionString, Environment.EventHubPath);
        }

        /// <summary>
        /// Sending entry and exit events to two different event hubs
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="entryHubName"></param>
        /// <param name="existHubName"></param>
        public static void SendData(string serviceBusConnectionString, string entryHubName, string existHubName)
        {
            var entryEventHub = EventHubClient.CreateFromConnectionString(serviceBusConnectionString, entryHubName);
            var exitEventHub = EventHubClient.CreateFromConnectionString(serviceBusConnectionString, existHubName);

            var timerInterval = TimeSpan.FromSeconds(1);
            var generator = GameDataEventGenerator.Generator();

            TimerCallback timerCallback = state =>
            {
                var startTime = DateTime.UtcNow;
                generator.Next(startTime, timerInterval, 5);

                foreach (var e in generator.GetEvents(startTime))
                {
                    if (e is EntryEvent)
                    {
                        Console.WriteLine("Sending start event data '{0}','{1}'", e.PlayerId, e.GameId);
                        entryEventHub.Send(
                           new EventData(Encoding.UTF8.GetBytes(e.Format()))
                           {
                               PartitionKey = e.GameId.ToString(CultureInfo.InvariantCulture)
                           });
                    }
                    else
                    {
                        Console.WriteLine("Sending stop event data '{0}','{1}'", e.PlayerId, e.GameId);
                        exitEventHub.Send(
                           new EventData(Encoding.UTF8.GetBytes(e.Format()))
                           {
                               PartitionKey = e.GameId.ToString(CultureInfo.InvariantCulture)
                           });
                    }
                }

                timer.Change((int)timerInterval.TotalMilliseconds, Timeout.Infinite);
            };

            timer = new Timer(timerCallback, null, Timeout.Infinite, Timeout.Infinite);
            timer.Change(0, Timeout.Infinite);

            Console.WriteLine("Sending event hub data... Press Ctrl+c to stop.");

            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Stopping...");
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();
            Console.WriteLine("Shutting down all resources...");
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            Thread.Sleep(timerInterval);
            timer.Dispose();
            entryEventHub.Close();
            exitEventHub.Close();
            Console.WriteLine("Stopped.");

        }


        /// <summary>
        /// Sending entry and exit game events to a single event hub
        /// </summary>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="eventHubName"></param>
        public static void SendData(string serviceBusConnectionString, string eventHubName)
        {
            var eventHub = EventHubClient.CreateFromConnectionString(serviceBusConnectionString, eventHubName);

            var timerInterval = TimeSpan.FromSeconds(1);
            var generator = GameDataEventGenerator.Generator();

            TimerCallback timerCallback = state =>
            {
                var startTime = DateTime.UtcNow;
                generator.Next(startTime, timerInterval, 5);

                foreach (var e in generator.GetEvents(startTime))
                {
                    if (e is EntryEvent)
                    {
                        Console.WriteLine("Sending start event data '{0}','{1}'", e.PlayerId, e.GameId);
                    }
                    else
                    {
                        Console.WriteLine("Sending stop event data '{0}','{1}'", e.PlayerId, e.GameId);
                    }
                    eventHub.Send(new EventData(Encoding.UTF8.GetBytes(e.Format()))
                    {
                        PartitionKey = e.GameId.ToString(CultureInfo.InvariantCulture)
                    });
                }

                timer.Change((int)timerInterval.TotalMilliseconds, Timeout.Infinite);
            };

            timer = new Timer(timerCallback, null, Timeout.Infinite, Timeout.Infinite);
            timer.Change(0, Timeout.Infinite);

            Console.WriteLine("Sending event hub data... Press Ctrl+c to stop.");

            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Stopping...");
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();
            Console.WriteLine("Shutting down all resources...");
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            Thread.Sleep(timerInterval);
            timer.Dispose();
            eventHub.Close();
            Console.WriteLine("Stopped.");

        }

    }
}
