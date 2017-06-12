// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Analytics
{
    public interface IFolderStructure
    {
        string[] GetFolders(string partitionId, string pipelineName, int index, Message msg, out string fileName);
    }
}
