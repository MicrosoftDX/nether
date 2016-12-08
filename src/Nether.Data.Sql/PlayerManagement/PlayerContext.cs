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
            PlayerEntity entity = await Players.FindAsync(player.PlayerId);
            if (entity == null)
            {
                await Players.AddAsync(new PlayerEntity
                {
                    PlayerId = player.PlayerId,
                    Gamertag = player.Gamertag,
                    Country = player.Country,
                    CustomTag = player.CustomTag,
                    PlayerImage = player.PlayerImage
                });
                await SaveChangesAsync();
            }        
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            return await Players.Select(p => new Player {
                PlayerId = p.PlayerId,
                Gamertag = p.Gamertag,
                Country = p.Country,
                CustomTag = p.CustomTag,
                PlayerImage = p.PlayerImage
            }).ToListAsync();
        }

        public async Task<Player> GetPlayerDetailsAsync(string gamertag)
        {
            var player = await Players.SingleAsync(p => p.Gamertag.Equals(gamertag));
            return new Player
            {
                PlayerId = player.PlayerId,
                Gamertag = player.Gamertag,
                Country = player.Country,
                CustomTag = player.CustomTag,
                PlayerImage = player.PlayerImage
            };
        }

        public async Task<byte[]> GetPlayerImageAsync(string gamertag)
        {
            var player = await Players.SingleAsync(p => p.Gamertag.Equals(gamertag));
            return player.PlayerImage;
        }

        public async Task<Player> GetPlayerDetailsByIdAsync(string id)
        {
            var player = await Players.SingleAsync(p => p.PlayerId.Equals(id));
            return new Player
            {
                PlayerId = player.PlayerId,
                Gamertag = player.Gamertag,
                Country = player.Country,
                CustomTag = player.CustomTag,
                PlayerImage = player.PlayerImage
            };
        }

        public async Task<string> GetPlayerIdForGamerTag(string gamertag)
        {
            var player =  await Players.SingleAsync(p => p.Gamertag == gamertag);
            return player.PlayerId;
            
        }

        public async Task UploadPlayerImageAsync(string gamertag, byte[] image)
        {
            var player = await Players.SingleAsync(p => p.Gamertag.Equals(gamertag));
            player.PlayerImage = image;
            Players.Update(player);
            await SaveChangesAsync();
        }
    }

    public class PlayerEntity
    {
        public Guid Id { get; set; }
        public string PlayerId { get; set; }
        public string Gamertag { get; set; }
        public string Country { get; set; }
        public string CustomTag { get; set; }
        public byte[] PlayerImage { get; set; }
    }
}
