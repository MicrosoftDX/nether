// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    /// <summary>
    /// Interface for synchronization providers for the job scheduler
    /// </summary>
    public interface ISynchronizationProvider
    {
        Task<Tuple<bool, string>> TryAcquireLeaseAsync(string jobId);
        Task RenewLeaseAsync(string jobId, string leaseId);
        Task ReleaseLeaseAsync(string detailedJobName, string leaseId);
    }
}
