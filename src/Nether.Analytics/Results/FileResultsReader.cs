// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nether.Analytics
{
    public class FileResultsReader : IResultsReader
    {
        private string _rootPath;
        private IMessageFormatter _serializer;

        public FileResultsReader()
        {
        }
        public FileResultsReader(IMessageFormatter serializer, IFilePathAlgorithm filePathAlgorithm, string rootPath = @"C:\")
        {
            _serializer = serializer;
            _rootPath = rootPath;
        }

        public IEnumerable<Message> GetLatest()
        {
            // we're basing this implementation on the metadata of files
            // which means that we'll iterate from the root folder, and go from there 
            // on up, based on last time modified

            var dir = new DirectoryInfo(_rootPath);

            if (!dir.Exists)
                throw new InvalidOperationException($"The path supplied is invalid, as it does not exist: {_rootPath}");

            var file = GetLatest(dir);

            // whoops, no file, let's return an empty list
            if (file == null) yield break;

            // looks like we got the file, read it to the end
            // TODO: consider passing a stream to the serializer, in case the file is too big?
            bool skipFirstLine = _serializer.IncludeHeaders;

            using (StreamReader sr = file.OpenText())
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    // some formatters support a header row, i.e. the CSV files, which means
                    // we need to skip the header
                    if (skipFirstLine)
                    {
                        skipFirstLine = false;
                        continue;
                    }
                    yield return _serializer.Parse(s);
                }
            }
        }

        private FileInfo GetLatest(DirectoryInfo directory)
        {
            if (directory == null) return null;

            var latest = directory.GetDirectories().OrderByDescending(x => x.LastWriteTimeUtc).FirstOrDefault();
            if (latest == null)
            {
                var lastFile = directory.GetFiles().OrderByDescending(x => x.LastWriteTimeUtc).FirstOrDefault();
                return lastFile;
            }
            else
            {
                return GetLatest(latest);
            }
        }
    }
}
