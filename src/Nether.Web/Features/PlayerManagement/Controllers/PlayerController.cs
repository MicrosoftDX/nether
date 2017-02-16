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
    /// Player management
    /// </summary>
    [Authorize(Roles = RoleNames.Player)]
    [Route("player")]
    public class PlayerController : Controller
    {
        private readonly IPlayerManagementStore _store;
        private readonly ILogger _logger;

        public PlayerController(IPlayerManagementStore store, ILogger<PlayerController> logger)
        {
            _store = store;
            _logger = logger;
        }

        /// <summary>
        /// Gets the player information from currently logged in user
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("")]
        public async Task<ActionResult> GetCurrentPlayer()
        {
            string gamertag = User.GetGamerTag();
            if (string.IsNullOrWhiteSpace(gamertag))
            {
                return NotFound();
            }

            // Call data store
            var player = await _store.GetPlayerDetailsByGamertagAsync(gamertag);
            if (player == null)
                return NotFound();

            // Return result
            return Ok(PlayerGetResponseModel.FromPlayer(player));
        }

        /// <summary>
        ///  Updates information about the current player
        /// </summary>
        /// <param name="player">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [HttpPut("")]
        public async Task<ActionResult> PutCurrentPlayer([FromBody]PlayerPutRequestModel player)
        {
            string userId = User.GetId();

            // TODO - need to prevent modifying gamertag

            // Update player
            await _store.SavePlayerAsync(
                new Player { UserId = userId, Gamertag = player.Gamertag, Country = player.Country, CustomTag = player.CustomTag });

            // Return result
            return NoContent();
        }

        /// <summary>
        /// Deletes the player information for the currently logged in user
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("")]
        public async Task<ActionResult> DeleteCurrentPlayer()
        {
            string gamertag = User.GetGamerTag();
            if (string.IsNullOrWhiteSpace(gamertag))
            {
                return NotFound();
            }

            // Call data store
            await _store.DeletePlayerDetailsAsync(gamertag);

            // Return result
            return NoContent();
        }

        /// <summary>
        /// Gets the extended player information from currently logged in user
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerStateGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("state")]
        public async Task<ActionResult> GetCurrentPlayerState()
        {
            string userId = User.GetId();

            // Call data store
            var player = await _store.GetPlayerDetailsExtendedAsync(userId);
            if (player == null)
                return NotFound();

            // Return result
            return Ok(PlayerStateGetResponseModel.FromPlayer(player));
        }


        /// <summary>
        /// Updates extended (e.g. JSON) information about the current player
        /// </summary>
        /// <param name="player">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [HttpPut("state")]
        public async Task<ActionResult> PutCurrentPlayerState([FromBody]PlayerStatePutRequestModel player)
        {
            string userId = User.GetId();

            // Update extended player information
            // Update player
            await _store.SavePlayerExtendedAsync(
                new PlayerState { UserId = userId, Gamertag = player.Gamertag, State = player.ExtendedInformation });

            // Return result
            return NoContent();
        }

        /// <summary>
        /// Gets the list of groups current player belongs to.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GroupListResponseModel))]
        [HttpGet("groups")]
        public async Task<ActionResult> GetPlayerGroups()
        {
            var gamertag = User.GetGamerTag();
            var groups = await _store.GetPlayersGroupsAsync(gamertag);

            return Ok(GroupListResponseModel.FromGroups(groups));
        }

        /// <summary>
        /// Adds currently logged in player to a group.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPut("groups/{groupName}")]
        public async Task<ActionResult> AddCurrentPlayerToGroup(string groupName)
        {
            var gamertag = User.GetGamerTag();
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
    }
}
