// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Nether.Data.Sql.Analytics
{
    public class YearlyDurationsEntity
    {
        public int Year { get; set; }
        public string DisplayName { get; set; }
        public long AverageGenericDuration { get; set; }
    }

}

