// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class BlobSynchronizationProvider : ISynchronizationProvider
    {
        private string _storageConnectionString;
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;
        private readonly string _containerName = "schedulerstate";

        public BlobSynchronizationProvider(string connectionString)
        {
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
        /// Tries to acquire a lease on the specific blob (called jobName)
        /// </summary>
        /// <param name="jobName">String, refers to the name of the blob</param>
        /// <returns>The leaseID</returns>
        public async Task<string> AcquireLeaseAsync(string jobName)
        {
            if (_container == null) await InitializeAsync();

            //try to get a lease for this job
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(jobName);

            //if the blob does not exist, just create an empty one
            if (!await blockBlob.ExistsAsync())
                await blockBlob.UploadTextAsync(string.Empty);


            return await blockBlob.AcquireLeaseAsync(null);
        }

        public async Task ReleaseLeaseAsync(string jobName, string leaseID)
        {
            if (_container == null) await InitializeAsync();

            if (string.IsNullOrEmpty(leaseID))
                throw new Exception("LeaseID should have a value");

            //try to release the lease for this job
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(jobName);

            await blockBlob.ReleaseLeaseAsync(AccessCondition.GenerateLeaseCondition(leaseID));
        }
    }
}
