// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Ingest
{
    /// <summary>
    /// Interface for state providers for the job scheduler
    /// </summary>
    public interface IJobStateProvider
    {
        Task<DateTime?> GetLastExecutionDatetimeAsync(string jobId);
        Task SetLastExecutionDateTimeAsync(string jobId, DateTime lastExecutionTime, string leaseId);
        Task DeleteEntryAsync(string jobIdWithSchedule);
    }
}
