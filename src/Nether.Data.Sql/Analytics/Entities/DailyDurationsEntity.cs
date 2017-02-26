// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Data.Sql.Analytics
{
    public class DailyDurationsEntity
    {
        public DateTime EventDate { get; set; }
        public string DisplayName { get; set; }
        public long AverageGenericDuration { get; set; }
    }

}

