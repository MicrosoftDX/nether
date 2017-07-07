// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Ingest
{
    public interface IOutputManager
    {
        Task OutputMessageAsync(string partitionId, string pipelineName, int index, Message msg);
        Task FlushAsync(string partitionId);
    }
}