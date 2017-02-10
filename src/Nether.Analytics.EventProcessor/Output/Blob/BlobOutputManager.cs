// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Nether.Analytics.EventProcessor.Output.Blob
{
    public class BlobOutputManager
    {
        private Dictionary<string, ConcurrentQueue<string>> _messageQueues = new Dictionary<string, ConcurrentQueue<string>>();

        private const string BlobNameFormat = "D6";
        private readonly string _containerName;
        private readonly Dictionary<string, string> _currentBlobs = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _currentFolders = new Dictionary<string, string>();
        private readonly Func<string, string> _getFolderStructureFunc;
        private readonly long _maxBlobSize;
        private readonly string _storageAccountConnectionString;
        private CloudBlobContainer _container;
        private bool _isConnected;

        public BlobOutputManager(string storageAccountConnectionString, string containerName,
            Func<string, string> getFolderStructureFunc, long maxBlobBlobSize)
        {
            _storageAccountConnectionString = storageAccountConnectionString;
            _containerName = containerName;
            _getFolderStructureFunc = getFolderStructureFunc;
            _maxBlobSize = maxBlobBlobSize;
        }

        public void QueueAppendToBlob(string gameEventType, params string[] lines)
        {
            ConcurrentQueue<string> queue;

            if (!_messageQueues.ContainsKey(gameEventType))
            {
                queue = new ConcurrentQueue<string>();
                _messageQueues[gameEventType] = queue;
            }
            else
            {
                queue = _messageQueues[gameEventType];
            }

            foreach (var line in lines)
            {
                queue.Enqueue(line);
            }
        }

        public void FlushWriteQueues()
        {
            Parallel.ForEach(_messageQueues, q =>
            {
                var gameEventType = q.Key;
                var queue = q.Value;

                var lines = new List<string>();
                string line;

                while (queue.TryDequeue(out line))
                {
                    lines.Add(line);
                }

                // Only flush queues that had messages
                if (lines.Count > 0)
                {
                    AppendToBlob(gameEventType, lines.ToArray());
                }
            });
        }

        public void AppendToBlob(string gameEventType, params string[] lines)
        {
            if (!_isConnected)
                ConnectToBlobContainer();

            var folderName = GetFolderForEventType(gameEventType);
            var lastUsedBlobName = GetBlobNameToUse(gameEventType, folderName);
            const string extension = ".csv";

            var name = string.IsNullOrEmpty(lastUsedBlobName) ? 0.ToString(BlobNameFormat) : lastUsedBlobName;
            // Set name to zeros if new blob
            var blob = _container.GetAppendBlobReference(folderName + name + extension);

            if (lastUsedBlobName == "")
            {
                CreateBlob(blob);
                _currentBlobs[gameEventType] = name;
            }

            // Append all lines into one string with \n as separator
            var builder = new StringBuilder();
            foreach (var line in lines)
                builder.Append(line + "\n");

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString()));

            try
            {
                blob.AppendBlock(stream,
                    accessCondition: AccessCondition.GenerateIfMaxSizeLessThanOrEqualCondition(_maxBlobSize));
                Console.WriteLine($"....Data written to: {blob.Uri}");
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == 412) // HttpStatusCode.PreconditionFailed
                {
                    // Blob has become too large, append same data to next blob
                    RolloverToNextBlob(gameEventType, name, folderName, extension, stream);
                }
                else
                {
                    throw;
                }
            }
        }

        private void ConnectToBlobContainer()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(_storageAccountConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            _container = cloudBlobClient.GetContainerReference(_containerName);
            _container.CreateIfNotExists();

            _isConnected = true;
        }

        private void CreateBlob(CloudAppendBlob blob)
        {
            Console.WriteLine($"....Creating new blob: {blob.Uri}");
            try
            {
                blob.CreateOrReplace(AccessCondition.GenerateIfNotExistsCondition());
            }
            catch (StorageException)
            {
            }
        }

        private string GetBlobNameToUse(string gameEventType, string folderName)
        {
            string cachedBlobName;

            if (_currentBlobs.TryGetValue(gameEventType, out cachedBlobName))
                return cachedBlobName;

            // Find "last" blobname in folder
            var lastBlobNameInFolder = GetLastBlobNameInFolder(folderName) ?? "";
            _currentBlobs[gameEventType] = lastBlobNameInFolder;

            return lastBlobNameInFolder;
        }

        private string GetFolderForEventType(string gameEventType)
        {
            // Get folder structure for this event type (from current UTC time)
            var proposedFolder = _getFolderStructureFunc(gameEventType);

            if (!_currentFolders.ContainsKey(gameEventType))
            {
                // This is the first time this game event has occurred,
                // save the proposed folder name
                _currentFolders[gameEventType] = proposedFolder;
                return proposedFolder;
            }

            var cachedFolder = _currentFolders[gameEventType];

            if (cachedFolder == proposedFolder) return proposedFolder;

            // We've moved to a new folder, make sure to reset blob name
            _currentBlobs.Remove(gameEventType);

            // Cache and use the new proposed folder
            _currentFolders[gameEventType] = proposedFolder;

            return proposedFolder;
        }

        private string GetLastBlobNameInFolder(string folderName)
        {
            var dir = _container.GetDirectoryReference(folderName);
            var blobs = dir.ListBlobs();

            var lastBlobPath = (from b in blobs
                                orderby b.Uri.AbsolutePath descending
                                select b.Uri.AbsolutePath).FirstOrDefault();

            if (lastBlobPath == "")
                return ""; // No blob found

            return Path.GetFileNameWithoutExtension(lastBlobPath);
        }

        private string GetNextBlobName(string name)
        {
            // Assume name has format xxxxxx_yyyyyy or nnnnnn
            // where xxxxxx and yyyyyy can be arbitary string
            // and nnnnnn is an integer representation

            var nameParts = name.Split('_');

            if (nameParts.Length == 1)
            {
                // name is of format xxxxxx or nnnnnn
                int n;

                if (int.TryParse(name, out n))
                    return (++n).ToString("D6");
                return $"{name}_{0:D6}";
            }

            // name is of format xxxxxx_yyyyyy
            var xxxxxx = string.Concat(nameParts.Take(nameParts.Length - 1));
            var yyyyyy = nameParts.Last();

            // recursive call to self
            return $"{xxxxxx}_{GetNextBlobName(yyyyyy)}";
        }

        private void RolloverToNextBlob(string gameEventType, string name, string folderName, string extension,
            MemoryStream stream)
        {
            var nextName = GetNextBlobName(name);
            _currentBlobs[gameEventType] = nextName;

            var nextBlob = _container.GetAppendBlobReference(folderName + nextName + extension);
            CreateBlob(nextBlob);
            Console.WriteLine($"....Rolling over to new blob");
            stream.Position = 0;
            nextBlob.AppendBlock(stream);
            Console.WriteLine($"....Data written to: {nextBlob.Uri}");
        }
    }
}