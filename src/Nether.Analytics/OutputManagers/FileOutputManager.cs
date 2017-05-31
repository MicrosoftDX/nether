// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class FileOutputManager : IOutputManager
    {
        private IMessageFormatter _serializer;
        private IFilePathAlgorithm _filePathAlgorithm;
        private string _rootPath;

        private static ConcurrentDictionary<string, SemaphoreSlim> s_semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
        public FileOutputManager(IMessageFormatter serializer, IFilePathAlgorithm filePathAlgorithm, string rootPath = @"C:\")
        {
            _serializer = serializer;
            _filePathAlgorithm = filePathAlgorithm;
            _rootPath = rootPath;
        }

        public async Task OutputMessageAsync(string partitionId, string pipelineName, int idx, Message msg)
        {
            var serializedMessage = $"{_serializer.Format(msg)}{Environment.NewLine}";

            var filePath = GetFilePath(pipelineName, idx, msg);

            var key = $"{pipelineName}_{msg.MessageType}_{msg.Version}_{msg.PartitionId}";

            var semaphore = s_semaphores.GetOrAdd(key, new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();

            try
            {
                if (_serializer.IncludeHeaders)
                {
                    await AppendMessageToFileWithHeaderAsync(serializedMessage, filePath);
                }
                else
                {
                    await AppendMessageToFileWithoutHeaderAsync(serializedMessage, filePath);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        public Task FlushAsync(string partitionId)
        {
            return Task.CompletedTask;
        }

        private string GetFilePath(string pipelineName, int idx, Message msg)
        {
            var fp = _filePathAlgorithm.GetFilePath(pipelineName, idx, msg);
            var fileName = $"{fp.Name}.{_serializer.FileExtension}";

            return Path.Combine(_rootPath, Path.Combine(fp.Hierarchy), fileName);
        }

        private async Task AppendMessageToFileAsync(string serializedMessage, string filePath)
        {
            // If we don't need to take into consideration of the header row in the files
            // just use the following simple implementation for writing to the file
            using (StreamWriter stream = File.AppendText(filePath))
            {
                await stream.WriteAsync(serializedMessage);
            }
        }

        private async Task AppendMessageToFileWithHeaderAsync(string serializedMessage, string filePath)
        {
            var tryAgain = true;

            do
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        await AppendMessageToFileAsync(serializedMessage, filePath);
                        tryAgain = false;
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        // append the header to the file
                        string header = $"{_serializer.Header}{Environment.NewLine}";
                        await AppendMessageToFileAsync(header, filePath);
                    }
                }
                catch (Exception)
                {
                    // it is possible that another thread is now creating the file and adding the header
                    // wait a while before continue to try and write the file
                    await Task.Delay(1000);
                }
            } while (tryAgain);
        }

        private async Task AppendMessageToFileWithoutHeaderAsync(string serializedMessage, string filePath)
        {
            var tryAgain = true;

            do
            {
                try
                {
                    await AppendMessageToFileAsync(serializedMessage, filePath);
                    tryAgain = false;
                }
                catch (DirectoryNotFoundException)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }
                catch (Exception)
                {
                    // it is possible that another thread is now creating the file
                    // wait a while before continue to try and write the file
                    await Task.Delay(1000);
                }
            } while (tryAgain);
        }
    }
}

