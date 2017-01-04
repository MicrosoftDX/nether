// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;
using Nether.Web.Utilities;
using Swashbuckle.SwaggerGen.Annotations;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;

//TO DO: The group and player Image type is not yet implemented. Seperate methods need to be implemented to upload a player or group image
//TODO: Add versioning support
//TODO: Add authentication

namespace Nether.Web.Features.PlayerManagement
{
    /// <summary>
    /// Player management
    /// </summary>
    [Route("api")]
    public class PlayerManagementController : Controller
    {
        private const string ControllerName = "PlayerManagement";
        private readonly IPlayerManagementStore _store;
        private readonly ILogger<PlayerManagementController> _log;

        public PlayerManagementController(IPlayerManagementStore store, ILogger<PlayerManagementController> log)
        {
            _store = store;
            _log = log;
        }

        // Implementation of the player API
        // There are two views:
        //  1. By Player
        //  2. Admininstration  

        /// <summary>
        /// Gets the player information from currently logged in user
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(PlayerGetResponseModel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "player not found")]
        [Authorize(Roles = RoleNames.Player)]
        [HttpGet("player")]
        public async Task<ActionResult> GetCurrentPlayer()
        {
            string userId = User.GetId();

            // Call data store
            var player = await _store.GetPlayerDetailsByUserIdAsync(userId);
            if (player == null) return NotFound();

            // Return result
            return Ok(PlayerGetResponseModel.FromPlayer(player));
        }

        /// <summary>
        /// Updates information about the current player
        /// </summary>
        /// <param name="player">Player data</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "Player updated successfully")]
        [Authorize(Roles = RoleNames.Player)]
        [Route("player")]
        [HttpPut]
        public async Task<ActionResult> PutCurrentPlayer([FromBody]PlayerPutRequestModel player)
        {
            string userId = User.GetId();

            // Update player
            await _store.SavePlayerAsync(
                new Player { UserId = userId, Gamertag = player.Gamertag, Country = player.Country, CustomTag = player.CustomTag });

            // Return result
            return new NoContentResult();
        }

        /// <summary>
        /// Creates or updates information about a player. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="newPlayer">Player data</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.Created, Description = "player created")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "user has no gamertag")]
        [Authorize(Roles = RoleNames.Admin)]
        [Route("players")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]PlayerPostRequestModel newPlayer)
        {
            if (string.IsNullOrWhiteSpace(newPlayer.Gamertag))
            {
                return base.BadRequest(); //TODO: return error info in body
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
            string location = Url.Action(
                nameof(GetPlayer),
                ControllerName,
                new { gamertag = player.Gamertag });
            return base.Created(location, new { gamertag = player.Gamertag });
        }

        /// <summary>
        /// Gets player information by player's gamer tag. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="gamertag">Gamer tag</param>
        /// <returns>Player information</returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(PlayerGetResponseModel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "player not found")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("players/{gamertag}")]
        public async Task<ActionResult> GetPlayer(string gamertag)
        {
            // Call data store
            var player = await _store.GetPlayerDetailsAsync(gamertag);
            if (player == null) return NotFound();

            // Return result
            return Ok(PlayerGetResponseModel.FromPlayer(player));
        }

        /// <summary>
        /// Gets all players
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(PlayerListGetResponseModel))]
        [Authorize(Roles = RoleNames.Admin)]
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

        /// <summary>
        /// Gets the list of groups current player belongs to.
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(GroupListResponseModel))]
        [Authorize(Roles = RoleNames.PlayerOrAdmin)]
        [HttpGet("player/groups")]
        public async Task<ActionResult> GetPlayerGroups()
        {
            return await GetPlayerGroups(User.GetGamerTag());
        }

        /// <summary>
        /// Gets the list of group a player belongs to.
        /// </summary>
        /// <param name="gamerTag">Player's gamertag.</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(GroupListResponseModel))]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("players/{gamerTag}/groups")]
        public async Task<ActionResult> GetPlayerGroups(string gamerTag)
        {
            // Call data store
            var groups = await _store.GetPlayersGroupsAsync(gamerTag);

            // Return result
            return Ok(GroupListResponseModel.FromGroups(groups));
        }

        // ********************************** THIS endpoint is a temporary measure to quickly unblock auth, but needs to be removed ***************************
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("EVIL/HELPER/tagfromid/{playerid}")]
        public async Task<ActionResult> EVIL_HELPER_GetTagFromPlayerId(string playerid)
        {
            // Call data store
            var player = await _store.GetPlayerDetailsByUserIdAsync(playerid);

            if (player == null)
            {
                return NotFound();
            }

            // Return result
            return Ok(player.Gamertag);
        }

        /// <summary>
        /// Adds player to a group.
        /// </summary>
        /// <param name="playerName">Player's gamer tag</param>
        /// <param name="groupName">Group name.</param>
        /// <returns></returns>
        [Route("players/{playerName}/groups/{groupName}")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPut]
        public async Task<ActionResult> AddPlayerToGroup(string playerName, string groupName)
        {
            Group group = await _store.GetGroupDetailsAsync(groupName);
            if (group == null)
            {
                _log.LogDebug("group '{0}' not found", groupName);
                return BadRequest();
            }

            Player player = await _store.GetPlayerDetailsAsync(playerName);
            if (player == null)
            {
                _log.LogDebug("player '{0}' not found", playerName);
                return BadRequest();
            }

            await _store.AddPlayerToGroupAsync(group, player);

            return Ok();
        }

        /// <summary>
        /// Adds currently logged in player to a group.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <returns></returns>
        [Route("player/groups/{groupName}")]
        [Authorize(Roles = RoleNames.PlayerOrAdmin)]
        [HttpPut]
        public async Task<ActionResult> AddCurrentPlayerToGroup(string groupName)
        {
            return await AddPlayerToGroup(User.GetGamerTag(), groupName);
        }

        /// <summary>
        /// Removes a player from a group. 
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="playerName">Player name</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "player is removed from the group successfully")]
        [Route("groups/{groupName}/players/{playerName}")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpDelete]
        public async Task<ActionResult> DeletePlayerFromGroup(string groupName, string playerName)
        {
            Player player = await _store.GetPlayerDetailsAsync(playerName);
            Group group = await _store.GetGroupDetailsAsync(groupName);

            await _store.RemovePlayerFromGroupAsync(group, player);

            return new NoContentResult();
        }

        //Implementation of the group API

        /// <summary>
        /// Creates a new group.
        /// </summary>
        /// <param name="group">Group object</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.Created, Description = "group created")]
        [Authorize(Roles = RoleNames.PlayerOrAdmin)]
        [Route("groups")]
        [HttpPost]
        public async Task<ActionResult> PostGroup([FromBody]GroupPostRequestModel group)
        {
            // Save group
            await _store.SaveGroupAsync(
                new Group
                {
                    Name = group.Name,
                    CustomType = group.CustomType,
                    Description = group.Description,
                    Members = group.Members
                }
            );

            // Return result
            string location = Url.Action(
                nameof(GetGroup),
                ControllerName,
                new { groupName = group.Name });
            return Created(location, new { groupName = group.Name });
        }

        /// <summary>
        /// Get list of all groups. You must be an administrator to perform this action.
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(GroupListResponseModel))]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("groups")]
        public async Task<ActionResult> GetGroupsAsync()
        {
            // Call data store
            var groups = await _store.GetGroupsAsync();

            // Return result
            return Ok(GroupListResponseModel.FromGroups(groups));
        }

        /// <summary>
        /// Gets group by name.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(GroupGetResponseModel))]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("groups/{groupName}", Name = "GetGroup")]
        public async Task<ActionResult> GetGroup(string groupName)
        {
            // Call data store
            var group = await _store.GetGroupDetailsAsync(groupName);
            if (group == null)
            {
                return NotFound();
            }

            // Format response model
            var resultModel = new GroupGetResponseModel
            {
                Group = group
            };

            // Return result
            return Ok(resultModel);
        }

        /// <summary>
        /// Gets the members of the group as player objects.
        /// </summary>
        /// <param name="groupName">Name of the group</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(GroupMemberResponseModel))]
        [Authorize(Roles = RoleNames.PlayerOrAdmin)]
        [HttpGet("groups/{groupName}/players")]
        public async Task<ActionResult> GetGroupPlayers(string groupName)
        {
            // Call data store
            List<string> gamertags = await _store.GetGroupPlayersAsync(groupName);

            // Format response model
            var resultModel = new GroupMemberResponseModel
            {
                Gamertags = gamertags
            };

            // Return result
            return Ok(resultModel);
        }

        /// <summary>
        /// Updates group information
        /// </summary>
        /// <param name="group">Group name</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "group updated successfully")]
        [Route("groups/{name}")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPut]
        public async Task<ActionResult> PutGroup([FromBody]GroupPostRequestModel group)
        {
            // Update group
            await _store.SaveGroupAsync(
                    new Group
                    {
                        Name = group.Name,
                        CustomType = group.CustomType,
                        Description = group.Description,
                        Members = group.Members
                    }
                );

            // Return result
            return new NoContentResult();
        }
    }
}
