// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Data.Sql.Analytics
{
    public class DailyActiveSessionsEntity
    {
        public DateTime EventDate { get; set; }
        public int ActiveSessions { get; set; }
    }

}

