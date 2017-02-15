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
        [HttpGet("player")]
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
        /// Gets the player information from currently logged in user
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("player")]
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerExtendedGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("playerextended")]
        public async Task<ActionResult> GetCurrentPlayerExtended()
        {
            string userId = User.GetId();

            // Call data store
            var player = await _store.GetPlayerDetailsExtendedAsync(userId);
            if (player == null)
                return NotFound();

            // Return result
            return Ok(PlayerExtendedGetResponseModel.FromPlayer(player));
        }

        /// <summary>
        ///  Updates information about the current player
        /// </summary>
        /// <param name="player">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [HttpPut("player")]
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
        /// Updates extended (e.g. JSON) information about the current player
        /// </summary>
        /// <param name="player">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [HttpPut("playerextended")]
        public async Task<ActionResult> PutCurrentPlayerExtended([FromBody]PlayerExtendedPutRequestModel player)
        {
            string userId = User.GetId();

            // Update extended player information
            // Update player
            await _store.SavePlayerExtendedAsync(
                new PlayerExtended { UserId = userId, Gamertag = player.Gamertag, ExtendedInformation = player.ExtendedInformation });

            // Return result
            return NoContent();
        }
    }
}
