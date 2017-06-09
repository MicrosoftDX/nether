// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    /// <summary>
    /// Interface for state providers for the job scheduler
    /// </summary>
    public interface IStateProvider
    {
        Task<DateTime?> GetLastExecutionDatetimeAsync(string detailedJobName);
        Task SetLastExecutionDateTimeAsync(string detailedJobName, DateTime dt, string leaseID);
        Task DeleteEntryAsync(string blobName, int code);
    }
}
