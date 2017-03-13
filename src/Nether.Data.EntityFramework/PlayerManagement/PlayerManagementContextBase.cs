// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using System;
using Nether.Data.PlayerManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Nether.Data.EntityFramework.PlayerManagement
{
    public abstract class PlayerManagementContextBase : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public DbSet<PlayerGroupEntity> PlayerGroups { get; set; }
        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<PlayerExtendedEntity> PlayersExtended { get; set; }

        public PlayerManagementContextBase(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PlayerGroupEntity>()
                .HasKey(pg => new { pg.GroupName, pg.Gamertag });
            builder.Entity<PlayerGroupEntity>()
                .HasOne(pg => pg.Player)
                .WithMany(p => p.PlayerGroups)
                .HasForeignKey(pg => pg.Gamertag);
            builder.Entity<PlayerGroupEntity>()
                .HasOne(pg => pg.Group)
                .WithMany(g => g.PlayerGroups)
                .HasForeignKey(pg => pg.GroupName);
            builder.Entity<PlayerGroupEntity>().Property(p => p.Gamertag).HasMaxLength(50);

            builder.Entity<PlayerEntity>()
                .HasKey(p => p.Gamertag);
            builder.Entity<PlayerEntity>()
                .HasAlternateKey(p => p.UserId);
            builder.Entity<PlayerEntity>().Property(p => p.UserId).HasMaxLength(50);
            builder.Entity<PlayerEntity>().Property(p => p.Gamertag).HasMaxLength(50);

            builder.Entity<PlayerExtendedEntity>()
                .HasKey(p => p.Gamertag);
            builder.Entity<PlayerExtendedEntity>().Property(p => p.Gamertag).HasMaxLength(50);

            builder.Entity<GroupEntity>()
                .HasKey(g => g.Name);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            builder.UseLoggerFactory(_loggerFactory);
        }
    }
}