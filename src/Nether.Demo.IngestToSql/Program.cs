// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using Microsoft.Azure.EventHubs.Processor;

using Microsoft.Azure.EventHubs.Processor;
using Nether.EventHubs;
using Nether.Ingest;
using Nether.SQLDatabase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace Nether.Demo.IngestToSql
{
    internal class Program
    {
        // Demo of Nether Ingest setup to
        // - Input from EventHub
        // - Using Custom JSON Message Format
        //   - where "event_name" identifies the type of event rather than the default one "type"
        //   - where no version number is contained in the messages, so using a static version number of 1.0.0
        // - Only using the default pipeline, meaning that all messages goes through the same simple pipeline
        // - Outputting to SQLDatabase

        private static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine(@"   _   _      _   _               ");
            Console.WriteLine(@"  | \ | | ___| |_| |__   ___ _ __ ");
            Console.WriteLine(@"  |  \| |/ _ \ __| '_ \ / _ \ '__|");
            Console.WriteLine(@"  | |\  |  __/ |_| | | |  __/ |   ");
            Console.WriteLine(@"  |_| \_|\___|\__|_| |_|\___|_|   ");
            Console.WriteLine(@"  Demo Ingest To Sql ");
            Console.WriteLine();

            var app = new ProgramEx();
            app.RunAsync().Wait();
        }
    }

    public class ProgramEx
    {
        private int _infoMax = 10;

        public async Task RunAsync()
        {
            // Check that all configurations are set before continuing

            if (!Config.Check())
            {
                // Exiting due to missing configuration
                Console.WriteLine("Press any key to continue");
                Console.ReadKey(true);
                return;
            }

            // Setup Listener. This will be the same for all pipelines we are building.
            var listenerConfig = new EventProcessorHostOptions
            {
                EventHubConnectionString = Config.Root[Config.NAH_EHLISTENER_CONNECTIONSTRING],
                EventHubPath = Config.Root[Config.NAH_EHLISTENER_EVENTHUBPATH],
                ConsumerGroupName = Config.Root[Config.NAH_EHLISTENER_CONSUMERGROUP],
                StorageConnectionString = Config.Root[Config.NAH_EHLISTENER_STORAGECONNECTIONSTRING],
                LeaseContainerName = Config.Root[Config.NAH_EHLISTENER_LEASECONTAINERNAME]
            };
            var listener = new EventHubsListener(listenerConfig, new EventProcessorOptions { MaxBatchSize = 1000, PrefetchCount = 30000 });

            // Setup Message Parser. By default we are using Nether JSON Messages
            // Setting up parser that knows how to parse those messages.
            var parser = new EventHubListenerMessageJsonParser
            {
                AllowDbgEnqueuedTime = true,
                CorruptMessageAsyncFunc = OnCorruptMessageAsync,
                MessageTypePropertyName = "type",
                UseStaticMessageVersion = true,
                StaticMessageVersion = "1.0.0"
            };

            // User a builder to create routing infrastructure for messages and the pipelines
            var builder = new MessageRouterBuilder();

            Dictionary<string, Tuple<SqlDbType, int>> columntoDatatypeMapping = new Dictionary<string, Tuple<SqlDbType, int>>()
            {
                {"event_time", Tuple.Create<SqlDbType, int>(SqlDbType.DateTime, 0 )},
                {"install_time", Tuple.Create<SqlDbType, int>(SqlDbType.DateTime, 0 )},
                {"enqueuedTimeUtc", Tuple.Create<SqlDbType, int>(SqlDbType.DateTime, 0 )},
                {"offline", Tuple.Create<SqlDbType, int>(SqlDbType.Int, 0 )},
                {"resource_change", Tuple.Create<SqlDbType, int>(SqlDbType.Int, 0 )},
                {"amount", Tuple.Create<SqlDbType, int>(SqlDbType.Int, 0 )}
            };

            var sqlOutputManager = new SQLDatabaseOutputManager(Config.Root[Config.NAH_AZURE_SQLUTPUTMANAGER_CONNECTIONSTRING], columntoDatatypeMapping, true);

            builder.DefaultPipeline
                .AddHandler(new UnixTimeToDateTimeMessageHandler("install_time", "event_time"))
                .OutputTo(sqlOutputManager);

            // Build all pipelines
            var router = builder.Build();

            // Attach the differeing parts of the message processor together
            var messageProcessor = new MessageProcessor<EventHubListenerMessage>(listener, parser, router, OnMessageProcessorInfoAsync);

            // The following method will never exit
            await messageProcessor.ProcessAndBlockAsync();
        }

        private Task OnCorruptMessageAsync(string msg)
        {
            ConsoleEx.WriteLine(ConsoleColor.Red, "Unparsable message was received and discarded");
            ConsoleEx.WriteLine(ConsoleColor.Gray, msg);
            return Task.CompletedTask;
        }

        private Task OnMessageProcessorInfoAsync(MessageProcessorInformation info)
        {
            while (info.MessagesPerSecond > _infoMax)
            {
                _infoMax *= 10;
            }

            Console.WriteLine(InfoBar("Msg/s", 10, info.MessagesPerSecond, _infoMax, 80) + " Tot: " + info.TotalMessages);

            return Task.CompletedTask;
        }

        private string InfoBar(string label, int labelWidth, double value, int max, int length)
        {
            var percentageFilled = value / max;
            var v = $"({value.ToString("N2")})";
            var adjustedLength = length - labelWidth - 7 - v.Length; // remove start and end characters from bar length "Msg/s [#####        ] 9999"

            var filledChars = (int)(percentageFilled * adjustedLength);
            var unfilledChars = adjustedLength - filledChars;

            return label.PadRight(labelWidth) + "[" + new string('=', filledChars) + v + new string(' ', unfilledChars) + "] " + max.ToString().PadLeft(4);
        }
    }
}
