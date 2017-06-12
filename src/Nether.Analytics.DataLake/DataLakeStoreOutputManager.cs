// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Management.DataLake.Store;
using Microsoft.Azure.Management.DataLake.Store.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nether.Analytics;
using System.Collections.Generic;

namespace Nether.Analytics.DataLake
{
    public class DataLakeStoreOutputManager : IOutputManager
    {
        private IMessageFormatter _serializer;
        private IFolderStructure _folderStructure;
        private string _subscriptionId;
        private string _dlsAccountName;

        private DataLakeStoreFileSystemManagementClient _dlsFileSystemClient;
        private ServiceClientCredentials _serviceClientCredentials;

        private PartitionedOutputBuffers _buffers = new PartitionedOutputBuffers();

        public bool IsAuthenticated
        {
            get
            {
                return _serviceClientCredentials != null;
            }
        }

        public DataLakeStoreOutputManager(IMessageFormatter serializer, IFolderStructure folderStructure, ServiceClientCredentials serviceClientCredentials, string subscriptionId, string dlsAccountName)
        {
            _serializer = serializer;
            _folderStructure = folderStructure;

            _serviceClientCredentials = serviceClientCredentials;

            _subscriptionId = subscriptionId;
            _dlsAccountName = dlsAccountName;

            _dlsFileSystemClient = new DataLakeStoreFileSystemManagementClient(serviceClientCredentials);
        }


        public Task OutputMessageAsync(string partitionId, string pipelineName, int index, Message msg)
        {
            // the output expects a new line each time we write something, so we can
            // just append the new line at the end of the serialized output
            var serializedMessage = $"{_serializer.Format(msg)}{Environment.NewLine}";

            var filePath = GetFilePath(partitionId, pipelineName, index, msg);

            _buffers.Append(partitionId, filePath, serializedMessage);

            return Task.CompletedTask;
        }

        public async Task FlushAsync(string partitionId)
        {
            foreach (var buffer in _buffers.PopBuffers(partitionId))
            {
                if (_serializer.IncludeHeaders)
                {
                    await AppendMessageToFileWithHeaderAsync(buffer.Value, buffer.Key);
                }
                else
                {
                    await AppendMessageToFileAsync(buffer.Value, buffer.Key);
                }
            }
        }

        private string GetFilePath(string partitionId, string pipelineName, int index, Message msg)
        {
            var folders = _folderStructure.GetFolders(partitionId, pipelineName, index, msg, out var fileName);

            var path = "/" + string.Join("/", folders) + "/";
            var fileNameWitExtension = $"{fileName}.{_serializer.FileExtension}";

            return path + fileNameWitExtension;
        }

        private async Task AppendMessageToFileAsync(string serializedMessage, string filePath)
        {
            // If we don't need to take into consideration of the header row in the files
            // just use the following simple implementation for writing to the file
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedMessage)))
            {
                await _dlsFileSystemClient.FileSystem.ConcurrentAppendAsync(_dlsAccountName, filePath, stream, AppendModeType.Autocreate, SyncFlag.CLOSE);
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
                        await _dlsFileSystemClient.FileSystem.ConcurrentAppendAsync(_dlsAccountName, filePath, stream, syncFlag: SyncFlag.CLOSE);
                    }

                    tryAgain = false;
                }
                catch (AdlsErrorException appendEx)
                {
                    if (!appendEx.Message.Contains("NotFound"))
                    {
                        // Unknown exception occurred while appending content to existing file
                        throw;
                    }

                    // Exception "Operation returned an invalid status code 'NotFound'"
                    // Meaning, we have to create a new file to append to

                    try
                    {
                        Console.WriteLine($"Creating new file {filePath}");
                        // Notice: Run the below two methods using synchronous versions in order to provide
                        // as much chance as possible for the two operations to be run just after eachother.

                        // The next operation could throw an exception if a race condition occurrs where
                        // two or more threads both found that the file was missing at the same time.
                        _dlsFileSystemClient.FileSystem.Create(_dlsAccountName, filePath, overwrite: false);

                        // Since the above operation would throw an exception if more than one thread
                        // tried to create the file, we can now be sure that the below operation will only
                        // be run by the thread that actually ended up creating the file. This doesn't
                        // mean that we can't end up in a situation where an additional thread sees the file
                        // and starts writing before we've had a chance to append the header row on this file.

                        //TODO: Fix the above described problem that can cause the Header Row to be written after another row

                        // Write headers to file
                        // TODO: Fix the coupling to the new line, but again, we expect this to be there
                        string header = $"{_serializer.Header}{Environment.NewLine}";

                        using (var headerStream = new MemoryStream(Encoding.UTF8.GetBytes(header)))
                        {
                            _dlsFileSystemClient.FileSystem.ConcurrentAppend(_dlsAccountName, filePath, headerStream, syncFlag: SyncFlag.CLOSE);
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
