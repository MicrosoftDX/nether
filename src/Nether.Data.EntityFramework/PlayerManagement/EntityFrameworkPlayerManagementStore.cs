// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nether.Data.PlayerManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.EntityFramework.PlayerManagement
{
    public class EntityFrameworkPlayerManagementStore : IPlayerManagementStore
    {
        private PlayerManagementContextBase _context;

        private readonly ILogger _logger;

        public EntityFrameworkPlayerManagementStore(PlayerManagementContextBase context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<EntityFrameworkPlayerManagementStore>();
        }

        public async Task<Player> GetPlayerDetailsByGamertagAsync(string gamertag)
        {
            if (string.IsNullOrEmpty(gamertag))
            {
                throw new ArgumentException($"{nameof(gamertag)} is required");
            }
            PlayerEntity player = await _context.Players.SingleOrDefaultAsync(p => p.Gamertag.Equals(gamertag));
            return player?.ToPlayer();
        }

        public async Task<Player> GetPlayerDetailsByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException($"{nameof(userId)} is required");
            }
            PlayerEntity player = await _context.Players.SingleOrDefaultAsync(p => p.UserId.Equals(userId));
            return player?.ToPlayer();
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            return await _context.Players.Select(p => p.ToPlayer()).ToListAsync();
        }

        public async Task SavePlayerAsync(Player player)
        {
            var existingPlayerForGamertag = await _context.Players.SingleOrDefaultAsync(p => p.Gamertag == player.Gamertag);
            if (existingPlayerForGamertag != null && existingPlayerForGamertag.UserId != player.UserId)
            {
                // Can't use a gamertag from another user
                _logger.LogDebug("Save player - gamertag already in use: UserId '{0}', Gamertag '{1}'", player.UserId, player.Gamertag);
                throw new Exception("gamertag is already in use");
            }

            // add only if the player does not exist
            PlayerEntity entity = player.UserId == null
                ? null
                : await _context.Players.SingleOrDefaultAsync(p => p.Gamertag == player.Gamertag);
            if (entity == null)
            {
                _logger.LogDebug("Add player: UserId '{0}', Gamertag '{1}'", player.UserId, player.Gamertag);
                await _context.Players.AddAsync(new PlayerEntity
                {
                    UserId = player.UserId,
                    Gamertag = player.Gamertag,
                    Country = player.Country,
                    CustomTag = player.CustomTag,
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                if (player.Gamertag != entity.Gamertag)
                {
                    // Can't change gamertag
                    _logger.LogDebug("Save player - cannot change gamertag: UserId '{0}', Gamertag '{1}'", player.UserId, player.Gamertag);
                    throw new Exception("Cannot change gamertag");
                }
                _logger.LogDebug("Update player: UserId '{0}', Gamertag '{1}'", player.UserId, player.Gamertag);
                entity.Gamertag = player.Gamertag;
                entity.Country = player.Country;
                entity.CustomTag = player.CustomTag;
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeletePlayerDetailsForUserIdAsync(string userid)
        {
            var entity = await _context.Players.SingleOrDefaultAsync(p => p.UserId == userid);
            _context.Players.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetPlayerStateByUserIdAsync(string userId)
        {
            PlayerExtendedEntity player = await _context.PlayersExtended.SingleOrDefaultAsync(p => p.UserId.Equals(userId));
            return player?.State;
        }

        public async Task SavePlayerStateByUserIdAsync(string userId, string state)
        {
            // add only of the playerextended does not exist
            PlayerExtendedEntity entity = await _context.PlayersExtended.FindAsync(userId);
            if (entity == null)
            {
                entity = new PlayerExtendedEntity();
                _context.PlayersExtended.Add(entity);
            }
            entity.UserId = userId;
            entity.State = state;
            await _context.SaveChangesAsync();
        }
    }
}
