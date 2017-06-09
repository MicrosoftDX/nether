// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Nether.Analytics
{
    /// <summary>
    /// Interface for synchronization providers for the job scheduler
    /// </summary>
    public interface ISynchronizationProvider
    {
        Task<string> AcquireLeaseAsync(string detailedJobName);
        Task ReleaseLeaseAsync(string detailedJobName, string leaseID);
    }
}
