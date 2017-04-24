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
using Nether.Web.Features.PlayerManagement.Models.PlayerAdmin;

//TO DO: The group and player Image type is not yet implemented. Seperate methods need to be implemented to upload a player or group image
//TODO: Add versioning support

namespace Nether.Web.Features.PlayerManagement
{
    /// <summary>
    /// Player management admin API
    /// </summary>
    [Authorize(Roles = RoleNames.Admin)]
    [Route("admin/players")]
    [NetherService("PlayerManagement")]
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
        /// Creates a player. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="newPlayer">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPost("")]
        [ReturnValidationFailureOnInvalidModelState]
        public async Task<ActionResult> Post([FromBody]PlayerPostRequestModel newPlayer)
        {
            // Save player
            var player = new Player
            {
                UserId = newPlayer.UserId,
                Gamertag = newPlayer.Gamertag,
                Country = newPlayer.Country,
                CustomTag = newPlayer.CustomTag
            };
            await _store.SavePlayerAsync(player);

            // Return result
            return CreatedAtRoute(nameof(GetPlayer), new { gamertag = player.Gamertag }, null);
        }

        /// <summary>
        /// Updates a player. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="gamertag">The gamertag</param>
        /// <param name="newPlayer">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPut("{gamertag}")]
        public async Task<IActionResult> Put([FromRoute]string gamertag, [FromBody]PlayerPutRequestModel newPlayer)
        {
            if (string.IsNullOrWhiteSpace(gamertag))
            {
                return this.ValidationFailed(new ErrorDetail("gamertag", "gamertag is required"));
            }

            var player = await _store.GetPlayerDetailsByGamertagAsync(gamertag);
            if (player == null)
            {
                return NotFound();
            }
            // Save player
            player.Country = newPlayer.Country;
            player.CustomTag = newPlayer.CustomTag;
            await _store.SavePlayerAsync(player);

            // Return result
            return Ok();
        }


        /// <summary>
        /// Gets player state. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="gamertag">Gamer tag</param>
        /// <returns>Player extended information</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerStateGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("{gamertag}/state")]
        public async Task<ActionResult> GetPlayerState(string gamertag)
        {
            // Call data store
            var state = await _store.GetPlayerStateByGamertagAsync(gamertag);
            if (state == null)
                return NotFound();

            // Return result
            return Ok(new PlayerStateGetResponseModel { Gamertag = gamertag, State = state });
        }

        /// <summary>
        /// Set player state. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="gamertag"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPut("{gamertag}/state")]
        public async Task<IActionResult> PostState([FromRoute] string gamertag, [FromBody]string state)
        {
            if (string.IsNullOrWhiteSpace(gamertag))
            {
                return this.ValidationFailed(new ErrorDetail("gamertag", "gamertag is required"));
            }

            // Save player extended information
            await _store.SavePlayerStateByGamertagAsync(gamertag, state);

            // Return result
            return CreatedAtRoute(nameof(GetPlayer), new { gamertag }, null);
        }
    }
}
