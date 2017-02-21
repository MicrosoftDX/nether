// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Azure.WebJobs.ServiceBus;
using Nether.Analytics.EventProcessor.Output.Blob;
using Nether.Analytics.EventProcessor.Output.EventHub;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;
using System.Threading;

namespace Nether.Analytics.EventProcessor
{
    /// <summary>
    /// Main class of the EventProcessor. This class has the required trigger(s) to
    /// get called by the WebJob SDK whenever there is a new Event to Process
    /// </summary>
    public static class GameEventReceiver
    {
        private static readonly GameEventRouter s_router;
        private static CloudBlobContainer _tmpContainer;
        private static CloudBlobContainer _outputContainer;
        private const string FullMessagesQueueName = "fullmessages";

        static GameEventReceiver()
        {
            // Read Environment Variables for configuration
            // TODO: Fix configuration to be in line with other configuration in Nether

            Console.WriteLine("Configuring GameEventReceiver (from Environment Variables");

            var storageAccountConnectionString = ConfigResolver.Resolve("NETHER_ANALYTICS_STORAGE_CONNECTIONSTRING");
            Console.WriteLine($"outputStorageAccountConnectionString: {storageAccountConnectionString}");

            var outputContainer = ConfigResolver.Resolve("NETHER_ANALYTICS_STORAGE_CONTAINER");
            if (string.IsNullOrWhiteSpace(outputContainer))
                outputContainer = "gameevents";
            Console.WriteLine($"outputContainer: {outputContainer}");

            var tmpContainer = ConfigResolver.Resolve("NETHER_ANALYTICS_STORAGE_TMP_CONTAINER");
            if (string.IsNullOrWhiteSpace(tmpContainer))
                tmpContainer = "tmp";
            Console.WriteLine($"tmpContainer: {tmpContainer}");

            var outputEventHubConnectionString = ConfigResolver.Resolve("NETHER_INTERMEDIATE_EVENTHUB_CONNECTIONSTRING");
            Console.WriteLine($"outputEventHubConnectionString: {outputEventHubConnectionString}");

            var outputEventHubName = ConfigResolver.Resolve("NETHER_INTERMEDIATE_EVENTHUB_NAME");
            Console.WriteLine($"outputEventHubName: {outputEventHubName}");

            string webJobDashboardAndStorageConnectionString = ConfigResolver.Resolve("NETHER_WEBJOB_DASHBOARD_AND_STORAGE_CONNECTIONSTRING");

            var maxBlobSize = 1024; // 10kB

            Console.WriteLine($"Max Blob Size: {maxBlobSize / 1024 / 1024}MB ({maxBlobSize}B)");
            Console.WriteLine();

            // Configure Blob Output
            var blobOutputManager = new BlobOutputManager(
                storageAccountConnectionString,
                webJobDashboardAndStorageConnectionString,
                tmpContainer,
                outputContainer,
                maxBlobSize);

            // Configure EventHub Output
            var eventHubOutputManager = new EventHubOutputManager(outputEventHubConnectionString, outputEventHubName);

            // Setup Handler to use above configured output managers
            var handler = new GameEventHandler(blobOutputManager, eventHubOutputManager);

            // Configure Router to switch handeling to correct method depending on game event type
            s_router = new GameEventRouter(GameEventHandler.ResolveEventType,
                GameEventHandler.UnknownGameEventFormatHandler,
                GameEventHandler.UnknownGameEventTypeHandler,
                handler.Flush);

            s_router.RegisterKnownGameEventTypeHandler("count/v1.0.0", handler.HandleCountEvent);
            s_router.RegisterKnownGameEventTypeHandler("game-heartbeat/v1.0.0", handler.HandleGameHeartbeat);
            s_router.RegisterKnownGameEventTypeHandler("game-start/v1.0.0", handler.HandleGameStartEvent);
            s_router.RegisterKnownGameEventTypeHandler("game-stop/v1.0.0", handler.HandleGameStopEvent);
            s_router.RegisterKnownGameEventTypeHandler("location/v1.0.0", handler.HandleLocationEvent);
            s_router.RegisterKnownGameEventTypeHandler("score/v1.0.0", handler.HandleScoreEvent);
            s_router.RegisterKnownGameEventTypeHandler("start/v1.0.0", handler.HandleStartEvent);
            s_router.RegisterKnownGameEventTypeHandler("stop/v1.0.0", handler.HandleStopEvent);
            s_router.RegisterKnownGameEventTypeHandler("generic/v1.0.0", handler.HandleGenericEvent);
            s_router.RegisterKnownGameEventTypeHandler("level-completed/v1.0.0", handler.HandleLevelCompletedEvent);
            s_router.RegisterKnownGameEventTypeHandler("level-start/v1.0.0", handler.HandleLevelStartEvent);

            // setup containers
            var cloudStorageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            _tmpContainer = cloudBlobClient.GetContainerReference(tmpContainer);
            _outputContainer = cloudBlobClient.GetContainerReference(outputContainer);

            var createTmpContainer = _tmpContainer.CreateIfNotExistsAsync();
            var createOutputContainer = _outputContainer.CreateIfNotExistsAsync();

            Task.WaitAll(createTmpContainer, createOutputContainer);
        }       

        public static void HandleBatch([EventHubTrigger("%NETHER_INGEST_EVENTHUB_NAME%")] EventData[] events)
        {
            var dequeueTime = DateTime.UtcNow;

            if (events.Length > 1)
                Console.WriteLine($"....Received batch of {events.Length} events");

            foreach (var ev in events)
            {
                var gameEventData = new GameEventData(ev, dequeueTime);
                s_router.HandleGameEvent(gameEventData);
            }

            s_router.Flush();
        }
        
        //public async static void ProcessFullBlobsAsync([QueueTrigger("fullmessages")] string blobUri)
        //{
        //    // wait for 5 seconds before processing the blob to allow any thread writing to it to finish 
        //    Thread.Sleep(5 * 1000);

        //    try
        //    {
        //        // copy the append blob to a blobk blob
        //        CloudAppendBlob blob = new CloudAppendBlob(new Uri(blobUri));
                
        //        var targetBlob = GetTargetBlockBlob(blob);
        //        await CopyBlobAsync(blob, targetBlob);

        //        await FlagAsCopiedAysnc(blob);
        //    }
        //    catch (StorageException e)
        //    {
        //        Console.WriteLine($"Failed to process full blob {blobUri} with expection: {e.Message}");
        //    }
        //}

        /// <summary>
        /// Time triggered function - goes over all the blobs in the tmp container ones marked as copied
        /// </summary>        
        public static async Task TimerJob([TimerTrigger("00:00:30")] TimerInfo timer)
        {
            Console.WriteLine("TimerJob triggered");
            foreach (IListBlobItem b in _tmpContainer.ListBlobs(null, true))
            {
                // list all blob in temp container, filter the ones that have "copied" metadata
                if (b.GetType() == typeof(CloudAppendBlob))
                {
                    CloudAppendBlob item = (CloudAppendBlob)b;
                    item.FetchAttributes();
                    if (item.Metadata.Keys.Contains("copied"))
                    {
                        Console.WriteLine($"Delete blob {item.Uri.ToString()}");
                        item.Delete(DeleteSnapshotsOption.IncludeSnapshots);
                    }
                    else
                    {
                        if (item.Metadata.Keys.Contains("full"))
                        {
                            var targetBlob = GetTargetBlockBlob(item);
                            await CopyBlobAsync(item, targetBlob);
                            await FlagAsCopiedAysnc(item);
                        }
                    }
                }
            }
        }

        private async static Task FlagAsCopiedAysnc(CloudAppendBlob blob)
        {            
            blob.FetchAttributes();            
            blob.Metadata["copied"] = DateTime.Now.ToString();           
            await blob.SetMetadataAsync();
        }

        private static CloudBlockBlob GetTargetBlockBlob(CloudAppendBlob source)
        {
            var name = source.Name;
            var target = _outputContainer.GetBlockBlobReference(name);
            return target;
        }

        private static async Task CopyBlobAsync(CloudAppendBlob source, CloudBlockBlob target)
        {
            Console.WriteLine($"Copying {source.Container.Name}/{source.Name} to {target.Container.Name}/{target.Name}");
            var sw = new Stopwatch();
            sw.Start();
            using (var sourceStream = await source.OpenReadAsync())
            {
                await target.UploadFromStreamAsync(sourceStream);                
            }
            
            sw.Stop();
            Console.WriteLine($"Copy operation finished in {sw.Elapsed.TotalSeconds} second(s)");          
        }
    }
}