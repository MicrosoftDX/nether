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
    public class IdentityContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IdentityContextOptions _options;

        public DbSet<UserEntity> Users { get; set; }

        public IdentityContext(ILoggerFactory loggerFactory, IdentityContextOptions options)
        {
            _options = options;
            _loggerFactory = loggerFactory;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserEntity>()
                .HasKey(u=> u.UserId);

            _options?.OnModelCreating?.Invoke(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseLoggerFactory(_loggerFactory);

            _options?.OnConfiguring?.Invoke(builder);
        }
    }

    public class IdentityContextOptions
    {
        public Action<ModelBuilder> OnModelCreating { get; set; }
        public Action<DbContextOptionsBuilder> OnConfiguring { get; set; }
    }
}