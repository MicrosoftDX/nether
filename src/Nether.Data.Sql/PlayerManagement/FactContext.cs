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
    public class FactContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _table;
        private readonly ILoggerFactory _loggerFactory;

        public DbSet<FactEntity> PlayerManagementFact { get; set; }
        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }

        public FactContext(string connectionString, string table,
            ILoggerFactory loggerFactory)
        {
            _connectionString = connectionString;
            _table = table;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FactEntity>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<FactEntity>().ForSqlServerToTable(_table);
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

            await PlayerManagementFact.AddAsync(new FactEntity
            {
                Player = dbPlayer,
                Group = dbGroup
            });
            await SaveChangesAsync();
        }

        public async Task<List<string>> GetGroupPlayersAsync(string groupName)
        {
            List<string> groupPlayersGamertags = await PlayerManagementFact
                .Where(fact => fact.Group.Name == groupName)
                .Select(fact => fact.Player.Gamertag)
                .AsNoTracking()
                .ToListAsync();

            return groupPlayersGamertags;
        }

        public async Task<List<string>> GetPlayerGroupsAsync(string gamerTag)
        {
            List<string> groupNames = await PlayerManagementFact
                .Where(fact => fact.Player.Gamertag == gamerTag)
                .Select(fact => fact.Group.Name)
                .ToListAsync();

            return groupNames;
        }

        public async Task RemovePlayerFromGroupAsync(string groupName, string playerId)
        {
            List<FactEntity> facts = await PlayerManagementFact
                .Where(fact => fact.Group.Name == groupName && fact.Player.PlayerId == playerId)
                .ToListAsync();

            RemoveRange(facts);
            await SaveChangesAsync();
        }

        public class FactEntity
        {
            public Guid Id { get; set; }

            public GroupEntity Group { get; set; }

            public PlayerEntity Player { get; set; }
        }

        public class GroupEntity
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string CustomType { get; set; }
            public string Description { get; set; }

            public List<FactEntity> Facts { get; set; }
        }

        public class PlayerEntity
        {
            public Guid Id { get; set; }
            public string PlayerId { get; set; }
            public string Gamertag { get; set; }
            public string Country { get; set; }
            public string CustomTag { get; set; }

            public List<FactEntity> Facts { get; set; }

            public Player ToPlayer()
            {
                return new Player
                {
                    PlayerId = PlayerId,
                    Gamertag = Gamertag,
                    Country = Country,
                    CustomTag = CustomTag
                };
            }
        }
    }
}