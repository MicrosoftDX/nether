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
    public class PlayerContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _table;

        public DbSet<PlayerEntity> Players { get; set; }

        public PlayerContext(string connectionString, string table)
        {
            _connectionString = connectionString;
            _table = table;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PlayerEntity>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<PlayerEntity>().ForSqlServerToTable(_table)
                .HasKey(p => p.PlayerId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString);
        }
    }
}
