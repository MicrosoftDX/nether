// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    /// <summary>
    /// Stores and retrieves details about last execution dateTime in the format yyyyMMdd-HHmm
    /// </summary>
    public class BlobJobStateProvider : IJobStateProvider
    {
        private string _storageConnectionString;
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;
        private readonly string _containerName = Constants.JobStateContainerName;

        public BlobJobStateProvider(string connectionString)
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
        /// <param name="jobId">Name of the job</param>
        /// <returns>DateTime of the last executed job or null if no entry found</returns>
        public async Task<DateTime?> GetLastExecutionDatetimeAsync(string jobId)
        {
            if (_container == null) await InitializeAsync();
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(jobId);

            //blob will probably have already been created by sync provider
            //but we're playing it safe :)
            if (await blockBlob.ExistsAsync())
            {
                //get the last execution time
                var str = await blockBlob.DownloadTextAsync();

                //empty entry
                if (string.IsNullOrWhiteSpace(str))
                    return null;

                try
                {
                    return DateTime.Parse(str, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
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
        /// <param name="jobId">Name of the job</param>
        /// <param name="lastExecutionTime">The last execution DateTime</param>
        /// <param name="leaseId">The leaseId for exclusive access to the blob</param>
        /// <returns></returns>
        public async Task SetLastExecutionDateTimeAsync(string jobId, DateTime lastExecutionTime, string leaseId)
        {
            if (_container == null) await InitializeAsync();
            var blob = _container.GetBlockBlobReference(jobId);
            await blob.UploadTextAsync(lastExecutionTime.ToString(), Encoding.UTF8, AccessCondition.GenerateLeaseCondition(leaseId), null, null);
        }

        /// <summary>
        /// Resets job state
        /// </summary>
        /// <param name="jobIdWithSchedule">BlobName to be deleted</param>
        /// <returns></returns>
        public async Task DeleteEntryAsync(string jobIdWithSchedule)
        {
            if (_container == null) await InitializeAsync();
            var blob = _container.GetBlockBlobReference(jobIdWithSchedule);

            try
            {
                await blob.DeleteAsync();
            }
            catch (StorageException)
            {
                // Try again after breaking any lease
                await blob.BreakLeaseAsync(null);
                await blob.DeleteAsync();
            }
        }
    }
}
