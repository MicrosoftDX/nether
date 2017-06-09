// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class BlobSynchronizationProvider : ISynchronizationProvider
    {
        private string _storageConnectionString;
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;
        private readonly string _containerName = Constants.SchedulerStateContainerName;

        public BlobSynchronizationProvider(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"{nameof(connectionString)} cannot be null");
            _storageConnectionString = connectionString;
        }


        /// <summary>
        /// Initialize the connection to the blob storage
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            _storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _container = _blobClient.GetContainerReference(_containerName);
            await _container.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Tries to acquire a lease on the specific blob (called detailedJobName)
        /// </summary>
        /// <param name="detailedJobName">String, refers to the name of the blob</param>
        /// <returns>The leaseID</returns>
        public async Task<string> AcquireLeaseAsync(string detailedJobName)
        {
            if (_container == null) await InitializeAsync();

            //try to get a lease for this job
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(detailedJobName);

            //if the blob does not exist, just create an empty one
            if (!await blockBlob.ExistsAsync())
                await blockBlob.UploadTextAsync(string.Empty);


            return await blockBlob.AcquireLeaseAsync(null);
        }

        /// <summary>
        /// Release the lease on the specified blob (called detailedJobName)
        /// </summary>
        /// <param name="detailedJobName"></param>
        /// <param name="leaseID"></param>
        /// <returns></returns>
        public async Task ReleaseLeaseAsync(string detailedJobName, string leaseID)
        {
            if (_container == null) await InitializeAsync();

            if (string.IsNullOrEmpty(leaseID))
                throw new Exception("LeaseID should have a value");

            //try to release the lease for this job
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(detailedJobName);

            await blockBlob.ReleaseLeaseAsync(AccessCondition.GenerateLeaseCondition(leaseID));
        }
    }
}
