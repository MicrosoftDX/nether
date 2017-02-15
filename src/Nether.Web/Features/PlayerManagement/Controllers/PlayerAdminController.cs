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
    [Route("admin")]
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
        /// Creates or updates information about a player. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="newPlayer">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPost("players")]
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
        /// Adds/Update extend data to a player. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="Player">Player data</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPost("playersextended")]
        public async Task<ActionResult> PostExtended([FromBody]PlayerExtendedPostRequestModel Player)
        {
            if (string.IsNullOrWhiteSpace(Player.Gamertag))
            {
                return BadRequest(); //TODO: return error info in body
            }

            // Save player extended information
            var player = new PlayerExtended
            {
                UserId = Player.UserId ?? Guid.NewGuid().ToString(),
                Gamertag = Player.Gamertag,
                ExtendedInformation = Player.ExtendedInformation
            };
            await _store.SavePlayerExtendedAsync(player);

            // Return result
            return CreatedAtRoute(nameof(GetPlayer), new { gamertag = player.Gamertag }, null);
        }

        /// <summary>
        /// Gets player information by player's gamer tag. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="gamertag">Gamer tag</param>
        /// <returns>Player information</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("players/{gamertag}", Name = nameof(GetPlayer))]
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
        /// Gets extended player information by player's gamer tag. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="gamertag">Gamer tag</param>
        /// <returns>Player extended information</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerExtendedGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("playersextended/{gamertag}")]
        public async Task<ActionResult> GetPlayerExtended(string gamertag)
        {
            // Call data store
            var player = await _store.GetPlayerDetailsExtendedAsync(gamertag);
            if (player == null)
                return NotFound();

            // Return result
            return Ok(PlayerExtendedGetResponseModel.FromPlayer(player));
        }

        /// <summary>
        /// Gets all players
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PlayerListGetResponseModel))]
        [HttpGet("players")]
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
    }
}
