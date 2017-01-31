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
    public class SqlIdentityContext : IdentityContextBase
    {
        private readonly SqlIdentityContextOptions _options;

        public SqlIdentityContext(ILoggerFactory loggerFactory, SqlIdentityContextOptions options)
            : base(loggerFactory)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserEntity>().ForSqlServerToTable("Users");
            builder.Entity<LoginEntity>().ForSqlServerToTable("UserLogins");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseSqlServer(_options.ConnectionString);
        }
    }

    public class SqlIdentityContextOptions
    {
        public string ConnectionString { get; set; }
    }
}