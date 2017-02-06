using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;

namespace Nether.Analytics.EventProcessor.Output.Blob
{
    public class BlobOutputManager
    {
        private readonly string _storageAccountConnectionString;
        private readonly string _containerName;
        private readonly Func<string> _folderStructureFunc;
        private readonly Dictionary<string, string> _blobNameDictionary = new Dictionary<string, string>();
        private readonly long _maxBlobSize;

        public BlobOutputManager(string storageAccountConnectionString, string containerName, Func<string> folderStructureFunc, long maxBlobBlobSize )
        {
            _storageAccountConnectionString = storageAccountConnectionString;
            _containerName = containerName;
            _folderStructureFunc = folderStructureFunc;
            _maxBlobSize = maxBlobBlobSize;
        }


        public void SendTo(string gameEventType, string data)
        {
            var account = CloudStorageAccount.Parse(_storageAccountConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(_containerName);
            var folder = _folderStructureFunc();
            var blobName = GetBlobName(gameEventType);
            var fullBlobName = $"{folder}/{blobName}";
            var blob = container.GetAppendBlobReference(fullBlobName);

            try
            {
                blob.AppendText(data, accessCondition: AccessCondition.GenerateIfMaxSizeLessThanOrEqualCondition(_maxBlobSize));
            }
            //TODO: Figure out exactly what exception is thrown if blob is too big and only catch that
            catch (Exception)
            {
                // Blob was too big, get new blob name
                GetNewBlobName(gameEventType);
                // ... and try again
                SendTo(gameEventType, data);
            }

            //TODO: Catch and handle other exceptions
        }


        private string GetBlobName(string gameEventType)
        {
            var name = _blobNameDictionary[gameEventType];
            return string.IsNullOrWhiteSpace(name) ? GetNewBlobName(gameEventType) : name;
        }

        private string GetNewBlobName(string gameEventType)
        {
            var name = $"{Guid.NewGuid()}.csv";
            _blobNameDictionary[gameEventType] = name;

            return name;
        }
    }
}