// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;
using Nether.Web.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using Nether.Web.Features.PlayerManagement.Models.PlayerManagement;

//TO DO: The group and player Image type is not yet implemented. Seperate methods need to be implemented to upload a player or group image
//TODO: Add versioning support

namespace Nether.Web.Features.PlayerManagement
{
    /// <summary>
    /// Player management admin API
    /// </summary>
    [Authorize(Roles = RoleNames.Admin)]
    [Route("admin/players")]
    public class PlayerAdminController : Controller
    {
        private readonly IPlayerManagementStore _store;
        private readonly ILogger _logger;

        public PlayerAdminController(IPlayerManagementStore store, ILogger<PlayerAdminController> logger)
        {
            _store = store;
            _logger = logger;
        }

        /// <summary>
        /// Gets all players
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerListGetResponseModel))]
        [HttpGet("")]
        public async Task<ActionResult> GetPlayers()
        {
            // Call data store
            var players = await _store.GetPlayersAsync();

            // Format response model
            var resultModel = new PlayerListGetResponseModel
            {
                Players = players.Select(p => (PlayerListGetResponseModel.PlayersEntry)p).ToList()
            };

            // Return result
            return Ok(resultModel);
        }

        /// <summary>
        /// Gets player information by player's gamer tag. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="gamertag">Gamer tag</param>
        /// <returns>Player information</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("{gamertag}", Name = nameof(GetPlayer))]
        public async Task<ActionResult> GetPlayer(string gamertag)
        {
            // Call data store
            var player = await _store.GetPlayerDetailsByGamertagAsync(gamertag);
            if (player == null)
                return NotFound();

            // Return result
            return Ok(PlayerGetResponseModel.FromPlayer(player));
        }

        /// <summary>
        /// Creates or updates information about a player. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="newPlayer">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPost("")]
        public async Task<ActionResult> Post([FromBody]PlayerPostRequestModel newPlayer)
        {
            if (string.IsNullOrWhiteSpace(newPlayer.Gamertag))
            {
                return BadRequest(); //TODO: return error info in body
            }

            // Save player
            var player = new Player
            {
                UserId = newPlayer.UserId ?? Guid.NewGuid().ToString(),
                Gamertag = newPlayer.Gamertag,
                Country = newPlayer.Country,
                CustomTag = newPlayer.CustomTag
            };
            await _store.SavePlayerAsync(player);

            // Return result
            return CreatedAtRoute(nameof(GetPlayer), new { gamertag = player.Gamertag }, null);
        }


        /// <summary>
        /// Gets extended player information by player's gamer tag. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="gamertag">Gamer tag</param>
        /// <returns>Player extended information</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerStateGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("{gamertag}/state")]
        public async Task<ActionResult> GetPlayerState(string gamertag)
        {
            // Call data store
            var player = await _store.GetPlayerDetailsExtendedAsync(gamertag);
            if (player == null)
                return NotFound();

            // Return result
            return Ok(PlayerStateGetResponseModel.FromPlayer(player));
        }

        /// <summary>
        /// Adds/Update extend data to a player. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="Player">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPost("{gamertag}/state")]
        public async Task<ActionResult> PostState([FromBody]PlayerStatePostRequestModel Player)
        {
            if (string.IsNullOrWhiteSpace(Player.Gamertag))
            {
                return BadRequest(); //TODO: return error info in body
            }

            // Save player extended information
            var player = new PlayerState
            {
                UserId = Player.UserId ?? Guid.NewGuid().ToString(),
                Gamertag = Player.Gamertag,
                State = Player.ExtendedInformation
            };
            await _store.SavePlayerExtendedAsync(player);

            // Return result
            return CreatedAtRoute(nameof(GetPlayer), new { gamertag = player.Gamertag }, null);
        }


        /// <summary>
        /// Gets the list of group a player belongs to.
        /// </summary>
        /// <param name="gamertag">Player's gamertag.</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GroupListResponseModel))]
        [HttpGet("{gamertag}/groups")]
        public async Task<ActionResult> GetPlayerGroups(string gamertag)
        {
            // Call data store
            var groups = await _store.GetPlayersGroupsAsync(gamertag);

            // Return result
            return Ok(GroupListResponseModel.FromGroups(groups));
        }


        /// <summary>
        /// Adds player to a group.
        /// </summary>
        /// <param name="gamertag">Player's gamer tag</param>
        /// <param name="groupName">Group name.</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPut("{gamertag}/groups/{groupName}")]
        public async Task<ActionResult> AddPlayerToGroup(string gamertag, string groupName)
        {
            Group group = await _store.GetGroupDetailsAsync(groupName);
            if (group == null)
            {
                _logger.LogWarning("group '{0}' not found", groupName);
                return BadRequest();
            }

            Player player = await _store.GetPlayerDetailsByGamertagAsync(gamertag);
            if (player == null)
            {
                _logger.LogWarning("player '{0}' not found", gamertag);
                return BadRequest();
            }

            await _store.AddPlayerToGroupAsync(group, player);

            return NoContent();
        }

        /// <summary>
        /// Removes a player from a group.
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="gamertag">Player name</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [HttpDelete("{gamertag}/groups/{groupName}")]
        public async Task<ActionResult> DeletePlayerFromGroup(string gamertag, string groupName)
        {
            Player player = await _store.GetPlayerDetailsByGamertagAsync(gamertag);
            Group group = await _store.GetGroupDetailsAsync(groupName);

            await _store.RemovePlayerFromGroupAsync(group, player);

            return NoContent();
        }
    }
}
