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
        private readonly string _containerName = Constants.JobStateContainerName;
        private TimeSpan _leaseTime;

        public BlobSynchronizationProvider(string connectionString) : this (connectionString, TimeSpan.FromSeconds(15))
        { }

        public BlobSynchronizationProvider(string connectionString, TimeSpan leaseTime)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"{nameof(connectionString)} cannot be null");
            _storageConnectionString = connectionString;

            if (TimeSpan.FromSeconds(15) > leaseTime || leaseTime > TimeSpan.FromSeconds(60))
                throw new ArgumentException("leaseTime need to be between 15 and 60 seconds", "leaseTime");

            _leaseTime = leaseTime;
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
        /// <param name="jobId">String, refers to the name of the blob</param>
        /// <returns>The leaseId</returns>
        public async Task<Tuple<bool, string>> TryAcquireLeaseAsync(string jobId)
        {
            var blockBlob = await GetLeaseBlobForJob(jobId);

            try
            {
                return Tuple.Create(true, await blockBlob.AcquireLeaseAsync(_leaseTime));
            }
            catch (StorageException)
            {
                // There is already a lease on the blob
                return Tuple.Create(false, "");
            }
        }


        public async Task RenewLeaseAsync(string jobId, string leaseId)
        {
            var blockBlob = await GetLeaseBlobForJob(jobId);

            await blockBlob.RenewLeaseAsync(AccessCondition.GenerateLeaseCondition(leaseId));
        }

        /// <summary>
        /// Release the lease on the specified blob (called detailedJobName)
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="leaseId"></param>
        /// <returns></returns>
        public async Task ReleaseLeaseAsync(string jobId, string leaseId)
        {
            if (_container == null) await InitializeAsync();

            if (string.IsNullOrEmpty(leaseId))
                throw new Exception("LeaseId should have a value");

            //try to release the lease for this job
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(jobId);

            await blockBlob.ReleaseLeaseAsync(AccessCondition.GenerateLeaseCondition(leaseId));
        }

        private async Task<CloudBlockBlob> GetLeaseBlobForJob(string jobId)
        {
            if (_container == null) await InitializeAsync();

            //try to get a lease for this job
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(jobId);

            //if the blob does not exist, just create an empty one
            if (!await blockBlob.ExistsAsync())
                await blockBlob.UploadTextAsync(string.Empty);
            return blockBlob;
        }
    }
}
