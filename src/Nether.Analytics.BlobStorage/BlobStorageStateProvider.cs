// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    /// <summary>
    /// Stores and retrieves details about last execution dateTime in the format yyyyMMdd-HHmm
    /// </summary>
    public class BlobStorageStateProvider : IStateProvider
    {
        private string _storageConnectionString;
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;
        private readonly string _containerName = Constants.SchedulerStateContainerName;

        public BlobStorageStateProvider(string connectionString)
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
        /// Gets the DateTime of the last executed job
        /// </summary>
        /// <param name="blobName">Name of the job</param>
        /// <returns>DateTime of the last executed job or null if no entry found</returns>
        public async Task<DateTime?> GetLastExecutionDatetimeAsync(string blobName)
        {
            if (_container == null) await InitializeAsync();
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(blobName);

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
                    DateTime dt = DateTimeUtilities.FromYMDHMString(datetimestring);
                    return dt;
                }
                //if we fail to get a valid datetimestring for any reason
                //e.g. the Lease provider just created the blob
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the last execution time for the job
        /// </summary>
        /// <param name="blobName">Name of the job</param>
        /// <param name="dt">The last execution DateTime</param>
        /// <param name="leaseID">The leaseID for exclusive access to the blob</param>
        /// <returns></returns>
        public async Task SetLastExecutionDateTimeAsync(string blobName, DateTime dt, string leaseID)
        {
            if (_container == null) await InitializeAsync();
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(blobName);
            string datestring = DateTimeUtilities.ToYMDHMSString(dt);
            await blockBlob.UploadTextAsync(datestring, Encoding.UTF8, AccessCondition.GenerateLeaseCondition(leaseID), null, null);
        }

        /// <summary>
        /// With the correct code, it will delete the blobName. Code needed for "make sure you're doing the right thing!" reason
        /// </summary>
        /// <param name="blobName">BlobName to be deleted</param>
        /// <param name="code">Answer to the Ultimate Question of Life, the Universe, and Everything</param>
        /// <returns></returns>
        public async Task DeleteEntryAsync(string blobName, int code)
        {
            if ((((code << 3) / 3) / 2) >> 2 == 14)
            {
                if (_container == null) await InitializeAsync();
                CloudBlockBlob blockBlob = _container.GetBlockBlobReference(blobName);
                await blockBlob.DeleteAsync();
            }
        }
    }
}
