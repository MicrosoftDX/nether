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
        private string _pipeline;
        private IFolderStructure _filePathAlgorithm;
        private string _messageType;

        public FileResultsReader()
        {
        }
        public FileResultsReader(IMessageFormatter serializer, IFolderStructure filePathAlgorithm,
            string rootPath, string pipeline, string messageType)
        {
            _serializer = serializer;
            _filePathAlgorithm = filePathAlgorithm;
            _rootPath = rootPath;
            _pipeline = pipeline;
            _messageType = messageType;
        }

        public IEnumerable<Message> GetLatest()
        {
            throw new NotImplementedException("Will be re-implemented in PR comming soon");

            //// we should get the root based on the pipeline and type, so we need to call the algorithm
            //var rootPath = _filePathAlgorithm.GetRootPath(_pipeline, _messageType);

            //rootPath = Path.Combine(_rootPath, rootPath);

            //// we're basing this implementation on the metadata of files
            //// which means that we'll iterate from the root folder, and go from there 
            //// on up, based on last time modified
            //var dir = new DirectoryInfo(rootPath);

            //if (!dir.Exists)
            //    throw new InvalidOperationException($"The path supplied is invalid, as it does not exist: {_rootPath}");

            //var file = GetLatest(dir);

            //return ReadFile(file);
        }

        public IEnumerable<Message> Get(DateTime from, DateTime to)
        {
            throw new NotImplementedException("Will be re-implemented in PR comming soon");

            //var paths = _filePathAlgorithm.GetFilePaths(_pipeline, _messageType, from, to);

            //foreach (var path in paths)
            //{
            //    var fullPath = Path.Combine(_rootPath, $"{path}.{_serializer.FileExtension}");
            //    string[] searchPaths = new string[] { fullPath };

            //    if (fullPath.Contains("*"))
            //    {
            //        // this is a search path, so let's do that
            //        // each file might have more than one partition
            //        var directoryName = Path.GetDirectoryName(fullPath);

            //        var di = new DirectoryInfo(directoryName);
            //        var searchPath = Path.GetFileName(fullPath);

            //        // the directory might actually not exist, in which case we can just skip it all
            //        if (!di.Exists) continue;

            //        var files = di.GetFiles(searchPath, SearchOption.TopDirectoryOnly);
            //        searchPaths = files.Select(x => x.FullName).ToArray();
            //    }

            //    // we need to look across multiple paths, because we might have 
            //    // had this filled out with the search patterns
            //    foreach (var actualPath in searchPaths)
            //    {
            //        foreach (var msg in ReadFile(new FileInfo(actualPath)))
            //        {
            //            yield return msg;
            //        }
            //    }
            //}
        }

        private IEnumerable<Message> ReadFile(FileInfo file)
        {
            // whoops, no file, let's return an empty list
            if (file == null) yield break;

            // if no file, don't panic
            if (!file.Exists) yield break;


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
