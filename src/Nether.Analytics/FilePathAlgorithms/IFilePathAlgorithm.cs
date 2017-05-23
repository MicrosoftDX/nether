// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics
{
    public interface IFilePathAlgorithm
    {
        FilePathResult GetFilePath(string pipelineName, int idx, Message msg);

        FilePathResult GetFilePath(string pipelineName, string messageType, DateTime dateTime);

        /// <summary>
        /// Returns a list of path results for a specific time span.
        /// </summary>
        /// <param name="pipelineName"></param>
        /// <param name="messageType"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IEnumerable<FilePathResult> GetFilePaths(string pipelineName, string messageType, DateTime start, DateTime end);
    }

    public struct FilePathResult
    {
        public string[] Hierarchy;
        public string Name;
    }
}
