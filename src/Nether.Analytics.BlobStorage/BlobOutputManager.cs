// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Ingest;
using System;
using System.Threading.Tasks;

namespace Nether.BlobStorage
{
    //TODO: Implement a working solution for outputting to Azure Blob Storage.
    // Take a look at the implementation for DataLakeStoreOutputManager for inspiration and sync
    public class BlobOutputManager : IOutputManager
    {
        private string _outputblobStorageConnectionString;

        public BlobOutputManager(string outputblobStorageConnectionString)
        {
            _outputblobStorageConnectionString = outputblobStorageConnectionString;
        }

        public Task OutputMessageAsync(string partitionId, string pipelineName, int index, Message msg)
        {
            throw new NotImplementedException();
        }

        public Task FlushAsync(string partitionId)
        {
            throw new NotImplementedException();
        }
    }
}