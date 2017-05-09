// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Management.DataLake.Store;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;
using Microsoft.Azure.Management.DataLake.Store.Models;
using System.Linq;

namespace Nether.Analytics.DataLake
{
    public class DataLakeStoreOutputManager : IOutputManager
    {
        private IOutputFormatter _serializer;
        private IFilePathAlgorithm _filePathAlgorithm;
        private ClientCredential _clientCredential;
        private string _domain;
        private string _subscriptionId;
        private string _adlsAccountName;

        //private DataLakeStoreAccountManagementClient _adlsClient;
        private DataLakeStoreFileSystemManagementClient _adlsFileSystemClient;
        private ServiceClientCredentials _serviceClientCredentials;

        public bool IsAuthenticated
        {
            get
            {
                return _serviceClientCredentials != null;
            }
        }

        public DataLakeStoreOutputManager(IOutputFormatter serializer, IFilePathAlgorithm filePathAlgorithm, string domain, string clientId, string clientSecret, string subscriptionId, string adlsAccountName)
            : this(serializer, filePathAlgorithm, domain, new ClientCredential(clientId, clientSecret), subscriptionId, adlsAccountName)
        {
        }

        public DataLakeStoreOutputManager(IOutputFormatter serializer, IFilePathAlgorithm filePathAlgorithm, string domain, ClientCredential clientCredential, string subscriptionId, string adlsAccountName)
        {
            _serializer = serializer;
            _filePathAlgorithm = filePathAlgorithm;

            _domain = domain;
            _clientCredential = clientCredential;

            _subscriptionId = subscriptionId;
            _adlsAccountName = adlsAccountName;
        }

        public DataLakeStoreOutputManager(IOutputFormatter serializer, IFilePathAlgorithm filePathAlgorithm, ServiceClientCredentials serviceClientCredentials, string subscriptionId, string adlsAccountName)
        {
            _serializer = serializer;
            _filePathAlgorithm = filePathAlgorithm;

            _serviceClientCredentials = serviceClientCredentials;

            _subscriptionId = subscriptionId;
            _adlsAccountName = adlsAccountName;

            _adlsFileSystemClient = new DataLakeStoreFileSystemManagementClient(_serviceClientCredentials);
        }


        public async Task OutputMessageAsync(string pipelineName, int idx, Message msg)
        {
            await CheckAuthentication();

            var serializedMessage = _serializer.Format(msg);
            var filePath = GetFilePath(pipelineName, idx, msg);

            if (_serializer.IncludeHeaderRow)
            {
                await AppendMessageToFileWithHeaderAsync(serializedMessage, filePath);
            }
            else
            {
                await AppendMessageToFileAsync(serializedMessage, filePath);
            }
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        private async Task CheckAuthentication()
        {
            if (!IsAuthenticated)
            {
                _serviceClientCredentials = await ApplicationTokenProvider.LoginSilentAsync(_domain, _clientCredential);
                _adlsFileSystemClient = new DataLakeStoreFileSystemManagementClient(_serviceClientCredentials);
            }
        }

        private string GetFilePath(string pipelineName, int idx, Message msg)
        {
            var fp = _filePathAlgorithm.GetFilePath(pipelineName, idx, msg);

            var path = "/" + string.Join("/", fp.hierarchy) + "/";
            var fileName = $"{fp.name}.{_serializer.FileExtension}";

            return path + fileName;
        }

        private async Task AppendMessageToFileAsync(string serializedMessage, string filePath)
        {
            // If we don't need to take into consideration of the header row in the files
            // just use the following simple implementation for writing to the file
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedMessage)))
            {
                await _adlsFileSystemClient.FileSystem.ConcurrentAppendAsync(_adlsAccountName, filePath, stream, AppendModeType.Autocreate, SyncFlag.CLOSE);
            }
        }

        private async Task AppendMessageToFileWithHeaderAsync(string serializedMessage, string filePath)
        {
            var tryAgain = true;

            do
            {
                try
                {
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedMessage)))
                    {
                        await _adlsFileSystemClient.FileSystem.ConcurrentAppendAsync(_adlsAccountName, filePath, stream, syncFlag: SyncFlag.CLOSE);
                    }

                    tryAgain = false;
                }
                catch (AdlsErrorException appendEx)
                {
                    if (!appendEx.Message.Contains("NotFound"))
                    {
                        // Unknown exception occurred wile appending content to existing file
                        throw;
                    }

                    // Exception "Operation returned an invalid status code 'NotFound'"
                    // Meaning, we have to create a new file to append to

                    try
                    {
                        // Notice: Run the below two methods using synchronous versions in order to provide
                        // as much chance as possible for the two operations to be run just after eachother.

                        // The next operation could throw an exception if a race condition occurrs where
                        // two or more threads both found that the file was missing at the same time.
                        _adlsFileSystemClient.FileSystem.Create(_adlsAccountName, filePath, overwrite: false);

                        // Since the above operation would throw an exception if more than one thread
                        // tried to create the file, we can now be sure that the below operation will only
                        // be run by the thread that actually ended up creating the file. This doesn't
                        // mean that we can't end up in a cituation where an additional thread sees the file
                        // and starts writing before we've had a chance to append the header row on this file.

                        //TODO: Fix the above described problem that can cause the Header Row to be written after another row

                        // Write headers to file
                        string header = _serializer.Header;

                        using (var headerStream = new MemoryStream(Encoding.UTF8.GetBytes(header)))
                        {
                            _adlsFileSystemClient.FileSystem.ConcurrentAppend(_adlsAccountName, filePath, headerStream, syncFlag: SyncFlag.CLOSE);
                        }
                    }
                    catch (AdlsErrorException createEx)
                    {
                        if (!createEx.Message.Contains("Forbidden"))
                        {
                            // Unknown exception was found while creating new file
                            throw;
                        }

                        // Exception "Operation returned an invalid status code 'Forbidden'"
                        // Meaning, file was created just after we figured out it didn't exist
                        // but before we managed to create it. Since another thread is creating
                        // the file and perhaps adding the headers, make sure to wait "a while"
                        // before continuing write operations. This condition should happen very
                        // rare and especially if new files are created seldom.

                        await Task.Delay(1000);
                    }
                }
            } while (tryAgain);
        }
    }
}
