// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics.DataLake
{
    public class BlobStorageStateProvider : IStateProvider
    {
        private string _storageConnectionString;
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;
        private readonly string _containerName = "schedulerstate";

        public BlobStorageStateProvider(string connectionString)
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
        /// Gets the DateTime of the last executed job
        /// </summary>
        /// <param name="jobName">Name of the job</param>
        /// <returns>DateTime of the last executed job or null if no entry found</returns>
        public async Task<DateTime?> GetLastExecutionDatetimeAsync(string jobName)
        {
            if (_container == null) await InitializeAsync();
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(jobName);

            //blob will probably have already been created by sync provider
            //but we're playing it safe :)
            if (await blockBlob.ExistsAsync())
            {
                //get the last execution time
                string datetimestring = await blockBlob.DownloadTextAsync();

                //empty entry
                if (string.IsNullOrEmpty(datetimestring.Trim()))
                    return null;

                try
                {
                    DateTime dt = DateTimeUtilities.FromSpecialString(datetimestring);
                    return dt;
                }
                //if we fail to get a valid datetimestring for any reason
                //e.g. the Lease provider just created the blob
                catch
                {
                    return null;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Sets the last execution time for the job
        /// </summary>
        /// <param name="jobName">Name of the job</param>
        /// <param name="dt">The last execution DateTime</param>
        /// <param name="leaseID">The leaseID for exclusive access to the blob</param>
        /// <returns></returns>
        public async Task SetLastExecutionDateTimeAsync(string jobName, DateTime dt, string leaseID)
        {
            if (_container == null) await InitializeAsync();
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(jobName);
            string datestring = DateTimeUtilities.ToSpecialString(dt);
            await blockBlob.UploadTextAsync(datestring, Encoding.UTF8, AccessCondition.GenerateLeaseCondition(leaseID), null, null);
        }
    }
}
