// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nether.Data.EntityFramework.PlayerManagement;

namespace Nether.Data.Sql.PlayerManagement
{
    public class SqlPlayerManagementContext : PlayerManagementContextBase
    {
        private readonly SqlPlayerManagementContextOptions _options;

        public SqlPlayerManagementContext(ILoggerFactory loggerFactory, SqlPlayerManagementContextOptions options)
            : base(loggerFactory)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PlayerGroupEntity>().ForSqlServerToTable("PlayerGroups");
            builder.Entity<PlayerEntity>().ForSqlServerToTable("Players");
            builder.Entity<GroupEntity>().ForSqlServerToTable("Groups");
            builder.Entity<PlayerExtendedEntity>().ForSqlServerToTable("PlayersExtended");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseSqlServer(_options.ConnectionString, options =>
            {
                options.MigrationsAssembly(this.GetType().GetTypeInfo().Assembly.GetName().Name);
                options.EnableRetryOnFailure();
            });
        }
    }

    public class SqlPlayerManagementContextOptions
    {
        public string ConnectionString { get; set; }
    }
}