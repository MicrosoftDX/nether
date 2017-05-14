// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics.PowerBI
{
    public class PowerBIOutputManager : IOutputManager
    {
        //TODO: Implement a working solution for outputting to PowerBI.
        // Take a look at the implementation for DataLakeStoreOutputManager for inspiration and sync
        public Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        public Task OutputMessageAsync(string pipelineName, int idx, Message msg)
        {
            throw new NotImplementedException();
        }
    }
}
