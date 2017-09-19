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

namespace Nether.Data.EntityFramework.Identity
{
    public abstract class IdentityContextBase : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<LoginEntity> Logins { get; set; }

        public IdentityContextBase(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserEntity>()
                .HasKey(u => u.UserId);
            builder.Entity<LoginEntity>()
                .HasOne(l => l.User)
                .WithMany(u => u.Logins)
                .HasForeignKey(l => l.UserId);

            builder.Entity<LoginEntity>()
                .HasKey(l => new { l.UserId, l.ProviderType });

            builder.Entity<UserEntity>().Property(u => u.UserId).HasMaxLength(50).IsRequired();
            builder.Entity<UserEntity>().Property(u => u.Role).HasMaxLength(50).IsRequired();

            builder.Entity<LoginEntity>().Property(l => l.UserId).HasMaxLength(50).IsRequired();
            builder.Entity<LoginEntity>().Property(l => l.ProviderType).HasMaxLength(50).IsRequired();
            builder.Entity<LoginEntity>().Property(l => l.ProviderId).HasMaxLength(50).IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseLoggerFactory(_loggerFactory);
        }
    }
}