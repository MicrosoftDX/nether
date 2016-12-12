// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System;

using Nether.Data.PlayerManagement;
using Nether.Web.Utilities;
using Swashbuckle.SwaggerGen.Annotations;
using Microsoft.Extensions.Logging;

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
            var player = await _store.GetPlayerDetailsByIdAsync(userId);
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
        public async Task<ActionResult> PutCurrentPlayer([FromBody]PlayerPostRequestModel player)
        {
            string userId = User.GetId();

            // Update player
            await _store.SavePlayerAsync(
                new Player { PlayerId = userId, Gamertag = player.Gamertag, Country = player.Country, CustomTag = player.CustomTag });

            // Return result
            return new NoContentResult();
        }

        /// <summary>
        /// Creates or updates information about a player. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="player">Player data</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.Created, Description = "player created")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "user has no gamer")]
        [Authorize(Roles = RoleNames.Admin)]
        [Route("players")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]PlayerPostRequestModel player)
        {
            if (string.IsNullOrWhiteSpace(player.Gamertag))
            {
                return BadRequest(); //TODO: return error info in body
            }

            // Save player
            await _store.SavePlayerAsync(new Player { Gamertag = player.Gamertag, Country = player.Country, CustomTag = player.CustomTag });

            // Return result
            string location = Url.Action(
                nameof(GetPlayer),
                ControllerName,
                new { gamerTag = player.Gamertag });
            return Created(location, new { gamerTag = player.Gamertag });
        }

        /// <summary>
        /// Gets player information by player's gamer tag. You have to be an administrator to perform this action.
        /// </summary>
        /// <param name="gamerTag">Gamer tag</param>
        /// <returns>Player information</returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(PlayerGetResponseModel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "player not found")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("players/{gamerTag}")]
        public async Task<ActionResult> GetPlayer(string gamerTag)
        {
            // Call data store
            var player = await _store.GetPlayerDetailsAsync(gamerTag);
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

        [Authorize(Roles = RoleNames.Player)]
        [HttpGet("player/groups/")]
        public Task<ActionResult> GetPlayerGroups()
        {
            return GetPlayerGroups(User.GetGamerTag());
        }

        [Route("player/groups/{groupname}")]
        [HttpPut]
        public async Task<ActionResult> AddPlayerToAGroup([FromBody]PlayerPostRequestModel playerin, string groupname)
        {
            //Get Player
            Player player = await _store.GetPlayerDetailsAsync(playerin.Gamertag);
            Group group = await _store.GetGroupDetailsAsync(groupname);

            // Save player
            await _store.AddPlayerToGroupAsync(group, player);

            // Return result
            return Ok();
        }

        [HttpGet("players/{gamerTag}/groups/")]
        public async Task<ActionResult> GetPlayerGroups(string gamerTag)
        {
            // Call data store
            var groups = await _store.GetPlayersGroupsAsync(gamerTag);

            // Format response model
            var resultModel = new GroupListResponseModel
            {
                Groups = groups.Cast<GroupListResponseModel.GroupsEntry>().ToList()
            };

            // Return result
            return Ok(resultModel);
        }

        // ********************************** THIS endpoint is a temporary measure to quickly unblock auth, but needs to be removed ***************************
        [HttpGet("EVIL/HELPER/tagfromid/{playerid}")]
        public async Task<ActionResult> EVIL_HELPER_GetTagFromPlayerId(string playerid)
        {
            // Call data store
            var player = await _store.GetPlayerDetailsByIdAsync(playerid);

            if (player == null)
            {
                return NotFound();
            }

            // Return result
            return Ok(player.Gamertag);
        }

        [Route("players/{playername}/groups/{groupname}")]
        [HttpPost]
        public async Task<ActionResult> AddPlayerToGroup(string playername, string groupname)
        {
            //Get Player
            Player player = await _store.GetPlayerDetailsAsync(playername);
            Group group = await _store.GetGroupDetailsAsync(groupname);

            // Save player
            await _store.AddPlayerToGroupAsync(group, player);

            // Return result
            return Ok();
        }



        //Implementation of the group API

        /// <summary>
        /// Creates a new group. You must be an administrator to perform this action.
        /// </summary>
        /// <param name="group">Group object</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.Created, Description = "group created")]
        [Authorize(Roles = RoleNames.Admin)]
        [Route("groups")]
        [HttpPost]
        public async Task<ActionResult> PostGroup([FromBody]GroupPostRequestModel group)
        {
            // Save group
            await _store.SaveGroupAsync(
                new Group
                {
                    Name = group.Name,
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

            // Format response model
            var resultModel = new GroupListResponseModel
            {
                Groups = groups.Select(s => (GroupListResponseModel.GroupsEntry)s).ToList()
            };

            // Return result
            return Ok(resultModel);
        }

        /// <summary>
        /// Gets the list of all groups
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

        [HttpGet("groups/{groupname}/players")]
        public async Task<ActionResult> GetGroupPlayers(string groupname)
        {
            // Call data store
            var players = await _store.GetGroupPlayersAsync(groupname);

            // Format response model
            var resultModel = new GroupMemberResponseModel
            {
                Members = players.Select(s => (GroupMemberResponseModel.PlayersEntry)s).ToList()
            };

            // Return result
            return Ok(resultModel);
        }

        [Route("groups/{groupname}/players/{playername}")]
        [HttpPost]
        public async Task<ActionResult> PostAddPlayerToGroup(string groupname, string playername)
        {
            Player player = await _store.GetPlayerDetailsAsync(playername);
            Group group = await _store.GetGroupDetailsAsync(groupname);

            // Save group
            await _store.AddPlayerToGroupAsync(group, player);

            // Return result
            return Ok();
        }


        [Route("groups/{groupname}/players/{playername}")]
        [HttpDelete]
        public async Task<ActionResult> DeletePlayerFromGroup(string groupname, string playername)
        {
            Player player = await _store.GetPlayerDetailsAsync(playername);
            Group group = await _store.GetGroupDetailsAsync(groupname);

            await _store.RemovePlayerFromGroupAsync(group, player);

            return new NoContentResult();
        }


        /// <summary>
        /// Updates group information
        /// </summary>
        /// <param name="group">Group object</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "group updated successfully")]
        [Route("groups/{group}")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPut]
        public async Task<ActionResult> PutGroup([FromBody]GroupPostRequestModel group)
        {
            // Update group
            await _store.SaveGroupAsync(
                    new Group
                    {
                        Name = group.Name,
                        Description = group.Description,
                        Members = group.Members
                    }
                );

            // Return result
            return new NoContentResult();
        }
    }
}
