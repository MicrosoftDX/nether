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
using Nether.Common.ApplicationPerformanceMonitoring;
using Microsoft.Net.Http.Headers;

//TO DO: The group and player Image type is not yet implemented. Seperate methods need to be implemented to upload a player or group image
//TODO: Add versioning support

namespace Nether.Web.Features.PlayerManagement
{
    /// <summary>
    /// Player management
    /// </summary>
    [Authorize(Roles = RoleNames.Player)]
    [Route("player")]
    [NetherService("PlayerManagement")]
    public class PlayerController : Controller
    {
        private readonly IPlayerManagementStore _store;
        private readonly ILogger _logger;
        private readonly IApplicationPerformanceMonitor _appMonitor;

        public PlayerController(
            IPlayerManagementStore store,
            IApplicationPerformanceMonitor appMonitor,
            ILogger<PlayerController> logger)
        {
            _appMonitor = appMonitor;
            _store = store;
            _logger = logger;
        }

        /// <summary>
        /// Gets the player information from currently logged in user
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerGetResponseModel))]
        [HttpGet("")]
        public async Task<ActionResult> GetCurrentPlayer()
        {
            string userId = User.GetId();

            // Call data store
            var player = await _store.GetPlayerDetailsByUserIdAsync(userId);
            if (player == null)
            {
                return Ok(new PlayerGetResponseModel { Player = new PlayerGetResponseModel.PlayerEntry() });
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

            bool newPlayer = false;
            var playerEntity = await _store.GetPlayerDetailsByUserIdAsync(userId);
            if (playerEntity == null)
            {
                newPlayer = true;
                if (player.Gamertag != null)
                {
                    // check if gamertag is already in use
                    var existingPlayerForGamertag = await _store.GetPlayerDetailsByUserIdAsync(player.Gamertag);
                    if (existingPlayerForGamertag != null && existingPlayerForGamertag.UserId != userId)
                    {
                        // Can't use a gamertag from another user
                        return this.ValidationFailed(new ErrorDetail("gamertag", "Gamertag already in use"));
                    }
                }
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

            // is this a new player registration?
            if (newPlayer)
            {
                _appMonitor.LogEvent("Register");
            }

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
            // Call data store
            await _store.DeletePlayerDetailsForUserIdAsync(User.GetId());

            // Return result
            return NoContent();
        }

        /// <summary>
        /// Gets the player state for the current player
        /// </summary>
        /// <returns></returns>
        [Produces("text/plain")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("state")]
        public async Task<ActionResult> GetCurrentPlayerState()
        {
            // Call data store
            var state = await _store.GetPlayerStateByUserIdAsync(User.GetId());

            // Return result
            return Content(state ?? "", new MediaTypeHeaderValue("text/plain"));
        }


        /// <summary>
        /// Updates state for the current player
        /// </summary>
        /// <param name="state">Player data</param>
        /// <returns></returns>
        [Consumes("text/plain")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPut("state")]
        public async Task<ActionResult> PutCurrentPlayerState([FromBody] string state) // TODO update binding to use raw string
        {
            // Update extended player information
            await _store.SavePlayerStateByUserIdAsync(User.GetId(), state);

            // Return result
            return Ok();
        }
    }
}
