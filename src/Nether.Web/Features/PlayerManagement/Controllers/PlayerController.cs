// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;
using Nether.Web.Utilities;
using Microsoft.Extensions.Logging;
using Nether.Web.Features.PlayerManagement.Models.Player;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
            {
                return Ok(new PlayerGetResponseModel());
            }

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
        public async Task<IActionResult> PutCurrentPlayer([FromBody]PlayerPutRequestModel player)
        {
            string userId = User.GetId();

            // prevent modifying gamertag
            var existingPlayerForGamertag = await _store.GetPlayerDetailsByGamertagAsync(player.Gamertag);
            if (existingPlayerForGamertag != null && existingPlayerForGamertag.UserId != userId)
            {
                // Can't use a gamertag from another user
                return this.ValidationFailed(new ErrorDetail("gamertag", "Gamertag already in use"));
            }

            var playerEntity = await _store.GetPlayerDetailsByUserIdAsync(userId);
            if (playerEntity == null)
            {
                playerEntity = new Player
                {
                    UserId = userId,
                    Gamertag = player.Gamertag,
                    Country = player.Country,
                    CustomTag = player.CustomTag
                };
            }
            else
            {
                if (playerEntity.Gamertag != player.Gamertag)
                {
                    // can't change gamertag
                    return this.ValidationFailed(new ErrorDetail("gamertag", "Can't change gamertag"));
                }
                playerEntity.Country = player.Country;
                playerEntity.CustomTag = player.CustomTag;
            }

            // Update player
            await _store.SavePlayerAsync(playerEntity);

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
        /// Gets the player state for the current player
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerStateGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("state")]
        public async Task<ActionResult> GetCurrentPlayerState()
        {
            string gamertag = User.GetGamerTag();

            // Call data store
            var state = await _store.GetPlayerStateByGamertagAsync(gamertag);

            // Return result
            return Ok(new PlayerStateGetResponseModel { Gamertag = gamertag, State = state });
        }


        /// <summary>
        /// Updates JSON state for the current player
        /// </summary>
        /// <param name="state">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPut("state")]
        public async Task<ActionResult> PutCurrentPlayerState([FromBody] JObject state) // TODO update binding to use raw string
        {
            string gamertag = User.GetGamerTag();

            // TODO - update this to use model binding. Keeping param for now for API docs, but binding to it isn't working
            var stateString = JsonConvert.SerializeObject(state);

            // Update extended player information
            // Update player
            await _store.SavePlayerStateByGamertagAsync(gamertag, stateString);

            // Return result
            return Ok();
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
                return NotFound();
            }

            Player player = await _store.GetPlayerDetailsByGamertagAsync(gamertag);
            if (player == null)
            {
                _logger.LogError("player '{0}' not found", gamertag);
                return BadRequest();
            }

            await _store.AddPlayerToGroupAsync(group, player);

            return NoContent();
        }
    }
}
