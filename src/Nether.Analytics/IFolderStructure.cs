// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics
{
    public interface IFilePathAlgorithm
    {
        FilePathResult GetFilePath(string partitionId, string pipelineName, int index, Message msg);
    }

    public struct FilePathResult
    {
        public string[] Hierarchy;
        public string Name;

        public override string ToString()
        {
            return System.IO.Path.Combine(string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), Hierarchy), Name);
        }
    }
}
