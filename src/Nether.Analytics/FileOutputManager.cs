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
        private string _rootPath;

        public FileOutputManager(IOutputFormatter serializer, IFilePathAlgorithm filePathAlgorithm, string rootPath = @"C:\")
        {
            _serializer = serializer;
            _filePathAlgorithm = filePathAlgorithm;
            _rootPath = rootPath;
        }

        public Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        public async Task OutputMessageAsync(string pipelineName, int idx, Message msg)
        {
            var serializedMessage = $"{_serializer.Format(msg)}{Environment.NewLine}";

            var filePath = GetFilePath(pipelineName, idx, msg);

            if (_serializer.IncludeHeaders)
            {
                await AppendMessageToFileWithHeaderAsync(serializedMessage, filePath);
            }
            else
            {
                await AppendMessageToFileWithoutHeaderAsync(serializedMessage, filePath);
                              
            }
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
                catch (Exception e)
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
                catch (DirectoryNotFoundException e)
                {                    
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));                   
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

