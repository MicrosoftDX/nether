// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Analytics
{
    public interface IFilePathAlgorithm
    {
        FilePathResult GetFilePath(string pipelineName, int idx, Message msg);
    }

    public struct FilePathResult
    {
        public string[] Hierarchy;
        public string Name;
    }
}
