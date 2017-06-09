// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    /// <summary>
    /// Interface that allows job scheduling
    /// </summary>
    public interface IJobSchedule
    {
        Task<IEnumerable<DateTime>> GetPendingExecutionsAsync(string detailedJobName);
        Task SetLastExecutionAsync(string detailedJobName, DateTime dt, string leaseID);
        JobInterval Interval { get; }
        string GetDetailedJobName(string shortJobName);
    }
}