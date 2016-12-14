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

        public async Task SavePlayerAsync(Player player)
        {
            // add only of the player does not exist
            PlayerEntity entity = player.PlayerId == null ? null : await Players.FindAsync(player.PlayerId);
            if (entity == null)
            {
                await Players.AddAsync(new PlayerEntity
                {
                    PlayerId = player.PlayerId,
                    Gamertag = player.Gamertag,
                    Country = player.Country,
                    CustomTag = player.CustomTag,
                });
                await SaveChangesAsync();
            }
            else
            {
                entity.Gamertag = player.Gamertag;
                entity.Country = player.Country;
                entity.CustomTag = player.CustomTag;
                await SaveChangesAsync();
            }
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            return await Players.Select(p => p.ToPlayer()).ToListAsync();
        }

        public async Task<Player> GetPlayerDetailsAsync(string gamertag)
        {
            PlayerEntity player = await Players.SingleOrDefaultAsync(p => p.Gamertag.Equals(gamertag));
            return player?.ToPlayer();
        }

        public Task<byte[]> GetPlayerImageAsync(string gamertag)
        {
            throw new NotSupportedException();
        }

        public async Task<Player> GetPlayerDetailsByIdAsync(string id)
        {
            PlayerEntity player = await Players.SingleOrDefaultAsync(p => p.PlayerId.Equals(id));
            return player?.ToPlayer();
        }

        public async Task<string> GetPlayerIdForGamerTag(string gamertag)
        {
            var player = await Players.SingleOrDefaultAsync(p => p.Gamertag == gamertag);
            return player?.PlayerId;
        }

        public Task UploadPlayerImageAsync(string gamertag, byte[] image)
        {
            throw new NotSupportedException();
        }

        public class PlayerEntity
        {
            public Guid Id { get; set; }
            public string PlayerId { get; set; }
            public string Gamertag { get; set; }
            public string Country { get; set; }
            public string CustomTag { get; set; }

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
