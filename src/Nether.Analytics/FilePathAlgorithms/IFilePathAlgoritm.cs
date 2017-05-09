// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Analytics
{
    public interface IFilePathAlgorithm
    {
        (string[] _hierarchy,string _name) GetFilePath(string pipelineName, int idx, Message msg);
    }
}
