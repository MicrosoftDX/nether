// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using System;
using Nether.Data.PlayerManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Nether.Data.Sql.Identity
{
    public class InMemoryIdentityContext : IdentityContextBase
    {
        public InMemoryIdentityContext(ILoggerFactory loggerFactory)
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