// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class FileOutputManager : IOutputManager
    {
        private IOutputFormatter _serializer;
        private IFilePathAlgorithm _filePathAlgorithm;

        public FileOutputManager(IOutputFormatter serializer, IFilePathAlgorithm filePathAlgorithm)
        {
            _serializer = serializer;
            _filePathAlgorithm = filePathAlgorithm;
        }

        public Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        public async Task OutputMessageAsync(string pipelineName, int idx, Message msg)
        {
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

        private string GetFilePath(string pipelineName, int idx, Message msg)
        {
            var fp = _filePathAlgorithm.GetFilePath(pipelineName, idx, msg);
            var fileName = $"{fp.Name}.{_serializer.FileExtension}";

            return string.Join("/", fp.Hierarchy) + fileName;
        }

        private async Task AppendMessageToFileAsync(string serializedMessage, string filePath)
        {
            // If we don't need to take into consideration of the header row in the files
            // just use the following simple implementation for writing to the file
            using (StreamWriter stream = File.AppendText(filePath))
            {
                await stream.WriteLineAsync(serializedMessage);
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
                        using (StreamWriter stream = File.AppendText(filePath))
                        {
                            await stream.WriteLineAsync(serializedMessage);
                        }

                        tryAgain = false;
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        string header = _serializer.Header;

                        using (StreamWriter stream = File.AppendText(filePath))
                        {
                            await stream.WriteLineAsync(header);
                        }
                    }
                }
                catch (Exception e)
                {
                    // it is possible that another thread is now creating the file and adding the header
                    // wait a while before continue to try and write the file
                    await Task.Delay(1000);
                }
            } while (tryAgain);
        }
    }
}

