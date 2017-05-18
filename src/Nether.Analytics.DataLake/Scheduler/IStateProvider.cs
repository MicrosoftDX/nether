// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics.DataLake
{
    public interface IStateProvider
    {
        Task<DateTime?> GetLastExecutionDatetimeAsync(string jobName);
        Task SetLastExecutionDateTimeAsync(string jobName, DateTime dt, string leaseID);
    }
}
