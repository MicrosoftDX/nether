// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nether.Data.PlayerManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Sql.PlayerManagement
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

        public async Task AddPlayerToGroupAsync(Group group, Player player)
        {
            // assuming that thhe player and the group already exist
            await AddPlayerToGroupAsync(group.Name, player.Gamertag);
        }

        private async Task AddPlayerToGroupAsync(string groupName, string gamertag)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentException($"{nameof(groupName)} is required");
            }
            if (string.IsNullOrEmpty(gamertag))
            {
                throw new ArgumentException($"{nameof(gamertag)} is required");
            }

            PlayerEntity dbPlayer = await _context.Players
                                                .Where(p => p.Gamertag == gamertag)
                                                .FirstOrDefaultAsync();

            if (dbPlayer == null)
                throw new ArgumentException($"player '{gamertag}' does not exist", nameof(gamertag));

            GroupEntity dbGroup = await _context.Groups
                                            .Where(g => g.Name == groupName)
                                            .FirstOrDefaultAsync();
            if (dbGroup == null)
                throw new ArgumentException($"group '{groupName}' does not exist", nameof(groupName));

            await _context.PlayerGroups.AddAsync(new PlayerGroupEntity
            {
                Gamertag = gamertag,
                GroupName = groupName
            });
            await _context.SaveChangesAsync();
        }

        public async Task<Group> GetGroupDetailsAsync(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentException($"{nameof(groupName)} is required");
            }

            var group = await _context.Groups.SingleAsync(g => g.Name.Equals(groupName));
            return new Group
            {
                Name = group.Name,
                CustomType = group.CustomType,
                Description = group.Description
            };
        }

        public async Task<byte[]> GetGroupImageAsync(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentException($"{nameof(groupName)} is required");
            }

            var group = await _context.Groups.SingleAsync(g => g.Name.Equals(groupName));
            return group.Image;
        }

        public async Task<List<string>> GetGroupPlayersAsync(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentException($"{nameof(groupName)} is required");
            }

            // get all the players for groupname
            List<string> groupPlayersGamertags = await _context.PlayerGroups
                .Where(map => map.Group.Name == groupName)
                .Select(map => map.Player.Gamertag)
                .AsNoTracking()
                .ToListAsync();

            return groupPlayersGamertags;
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await _context.Groups.Select(g => new Group
            {
                Name = g.Name,
                CustomType = g.CustomType,
                Description = g.Description
            }).ToListAsync();
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

        public Task<byte[]> GetPlayerImageAsync(string gamertag)
        {
            throw new NotSupportedException();
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            return await _context.Players.Select(p => p.ToPlayer()).ToListAsync();
        }

        public async Task<List<Group>> GetPlayersGroupsAsync(string gamertag)
        {
            if (string.IsNullOrEmpty(gamertag))
            {
                throw new ArgumentException($"{nameof(gamertag)} is required");
            }

            // get all the group names for player
            List<string> playerGroups = await GetPlayerGroupsAsync(gamertag);

            return playerGroups.Select(g => GetGroupDetailsAsync(g).Result).ToList();
        }
        private async Task<List<string>> GetPlayerGroupsAsync(string gamertag)
        {
            if (string.IsNullOrEmpty(gamertag))
            {
                throw new ArgumentException($"{nameof(gamertag)} is required");
            }
            List<string> groupNames = await _context.PlayerGroups
                .Where(map => map.Gamertag == gamertag)
                .Select(map => map.Group.Name)
                .ToListAsync();

            return groupNames;
        }

        public async Task RemovePlayerFromGroupAsync(Group group, Player player)
        {
            var playerGroups = await _context.PlayerGroups
                                                .Where(map => map.Group.Name == group.Name && map.Gamertag == player.Gamertag)
                                                .ToListAsync();

            _context.RemoveRange(playerGroups);
            await _context.SaveChangesAsync();
        }

        public async Task SaveGroupAsync(Group group)
        {
            // add a new group if does not exist
            await SaveGroupEntityAsync(group);

            // add a new player if does not exist and update the player<->group mapping table with the relation between player and group
            if (group.Members != null)
            {
                foreach (string playerGamerTag in group.Members)
                {
                    await AddPlayerToGroupAsync(group.Name, playerGamerTag);
                }
            }
        }
        public async Task SaveGroupEntityAsync(Group group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            // add new group only if it does not exist
            GroupEntity entity = await _context.Groups.FindAsync(group.Name);
            if (entity == null)
            {
                var newGroup = new GroupEntity
                {
                    Name = group.Name,
                    CustomType = group.CustomType,
                    Description = group.Description
                };

                await _context.Groups.AddAsync(newGroup);
                await _context.SaveChangesAsync();
                entity = newGroup;
            }
            else
            {
                entity.CustomType = group.CustomType;
                entity.Description = group.Description;
                await _context.SaveChangesAsync();
            }
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
        public async Task DeletePlayerDetailsAsync(string gamertag)
        {
            var entity = await _context.Players.SingleOrDefaultAsync(p => p.Gamertag == gamertag);
            _context.Players.Remove(entity);
            await _context.SaveChangesAsync();
        }


        public async Task UploadGroupImageAsync(string groupname, byte[] image)
        {
            var group = await _context.Groups.SingleAsync(g => g.Name.Equals(groupname));
            group.Image = image;
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        public Task UploadPlayerImageAsync(string gamertag, byte[] image)
        {
            throw new NotSupportedException();
        }

        public async Task SavePlayerStateByGamertagAsync(string gamertag, string state)
        {
            // add only of the playerextended does not exist
            PlayerExtendedEntity entity = await _context.PlayersExtended.FindAsync(gamertag);
            if (entity == null)
            {
                entity = new PlayerExtendedEntity();
                _context.PlayersExtended.Add(entity);
            }
            entity.Gamertag = gamertag;
            entity.State = state;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetPlayerStateByGamertagAsync(string gamertag)
        {
            PlayerExtendedEntity player = await _context.PlayersExtended.SingleOrDefaultAsync(p => p.Gamertag.Equals(gamertag));
            return player?.State;
        }
    }
}
