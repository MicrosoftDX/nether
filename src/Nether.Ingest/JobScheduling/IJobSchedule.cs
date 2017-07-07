// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nether.Ingest
{
    public interface IJobScheduleV2
    {
        DateTime GetNextExecutionTime(DateTime lastExecutionTime);
    }
}