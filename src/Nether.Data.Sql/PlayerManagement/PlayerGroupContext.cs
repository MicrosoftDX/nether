// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using System;
using Nether.Data.PlayerManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Nether.Data.Sql.PlayerManagement
{
    public class PlayerGroupContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _table;
        private readonly ILoggerFactory _loggerFactory;

        public DbSet<PlayerGroupEntity> PlayerGroups { get; set; }
        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }

        public PlayerGroupContext(string connectionString, string table,
            ILoggerFactory loggerFactory)
        {
            _connectionString = connectionString;
            _table = table;
            _loggerFactory = loggerFactory;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PlayerGroupEntity>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<PlayerGroupEntity>().ForSqlServerToTable(_table);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseSqlServer(_connectionString);
            builder.UseLoggerFactory(_loggerFactory);
        }
    }
}