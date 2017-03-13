// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nether.Data.EntityFramework.Analytics;

namespace Nether.Data.InMemory.Analytics
{
    public class InMemoryAnalyticsContext : AnalyticsContextBase
    {
        public InMemoryAnalyticsContext(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseInMemoryDatabase();
        }
    }
}