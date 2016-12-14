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

        public async Task AddPlayerToGroupAsync(Group group, string gamerTag)
        {
            PlayerEntity dbPlayer = await Players.Where(p => p.Gamertag == gamerTag).FirstOrDefaultAsync();
            if (dbPlayer == null) throw new ArgumentException($"player '{gamerTag}' does not exist", nameof(gamerTag));

            GroupEntity dbGroup = await Groups.Where(g => g.Name == group.Name).FirstOrDefaultAsync();
            if (dbGroup == null) throw new ArgumentException($"group '{group.Name}' does not exist", nameof(group));

            await PlayerGroups.AddAsync(new PlayerGroupEntity
            {
                Player = dbPlayer,
                Group = dbGroup
            });
            await SaveChangesAsync();
        }

        public async Task<List<string>> GetGroupPlayersAsync(string groupName)
        {
            List<string> groupPlayersGamertags = await PlayerGroups
                .Where(map => map.Group.Name == groupName)
                .Select(map => map.Player.Gamertag)
                .AsNoTracking()
                .ToListAsync();

            return groupPlayersGamertags;
        }

        public async Task<List<string>> GetPlayerGroupsAsync(string gamerTag)
        {
            List<string> groupNames = await PlayerGroups
                .Where(map => map.Player.Gamertag == gamerTag)
                .Select(map => map.Group.Name)
                .ToListAsync();

            return groupNames;
        }

        public async Task RemovePlayerFromGroupAsync(string groupName, string playerId)
        {
            List<PlayerGroupEntity> playerGroups = await PlayerGroups
                .Where(map => map.Group.Name == groupName && map.Player.PlayerId == playerId)
                .ToListAsync();

            RemoveRange(playerGroups);
            await SaveChangesAsync();
        }
    }
}