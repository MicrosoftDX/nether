using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Nether.Analytics.EventProcessor.Output.Blob
{
    public class BlobOutputManager
    {
        private static readonly Dictionary<string, string> BlobNameDictionary = new Dictionary<string, string>();

        private readonly string _storageAccountConnectionString;
        private readonly string _containerName;
        private readonly Func<string> _folderStructureFunc;
        private readonly long _maxBlobSize;

        public BlobOutputManager(string storageAccountConnectionString, string containerName, Func<string> folderStructureFunc, long maxBlobBlobSize )
        {
            _storageAccountConnectionString = storageAccountConnectionString;
            _containerName = containerName;
            _folderStructureFunc = folderStructureFunc;
            _maxBlobSize = maxBlobBlobSize;
        }


        public void AppendLineToBlob(string gameEventType, string data)
        {
            var account = CloudStorageAccount.Parse(_storageAccountConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(_containerName);
            container.CreateIfNotExists();
            var folder = $"{gameEventType}/{_folderStructureFunc()}";
            var blobName = GetBlobName(gameEventType);
            var fullBlobName = $"{folder}/{blobName}";
            var blob = container.GetAppendBlobReference(fullBlobName);

            try
            {

                if (!blob.Exists())
                {
                    Console.WriteLine($"Creating AppendBlob: {fullBlobName}");
                    try
                    {
                        blob.CreateOrReplace(AccessCondition.GenerateIfNotExistsCondition());
                    }
                    catch (StorageException ex)
                    {
                    }
                }

                blob.AppendText(data + "\n",accessCondition: AccessCondition.GenerateIfMaxSizeLessThanOrEqualCondition(_maxBlobSize));
            }
            //TODO: Figure out exactly what exception is thrown if blob is too big and only catch that
            catch (StorageException ex)
            {
                // Blob was too big, get new blob name
                GetNewBlobName(gameEventType);
                // ... and try again
                AppendLineToBlob(gameEventType, data);
            }

            //TODO: Catch and handle other exceptions
        }


        private string GetBlobName(string gameEventType)
        {
            return BlobNameDictionary.ContainsKey(gameEventType) ? BlobNameDictionary[gameEventType] : GetNewBlobName(gameEventType);
        }

        private string GetNewBlobName(string gameEventType)
        {
            var name = $"{Guid.NewGuid()}.csv";
            BlobNameDictionary[gameEventType] = name;

            return name;
        }
    }
}