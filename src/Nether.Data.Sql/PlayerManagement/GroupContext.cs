// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;

namespace Nether.Data.Sql.PlayerManagement
{
    public class GroupContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _table;

        public DbSet<GroupEntity> Groups { get; set; }

        public GroupContext(string connectionString, string table)
        {
            _connectionString = connectionString;
            _table = table;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GroupEntity>()
            .Property(g => g.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<GroupEntity>().ForSqlServerToTable(_table)
                .HasKey(g => g.Name);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString);
        }
    }
}
