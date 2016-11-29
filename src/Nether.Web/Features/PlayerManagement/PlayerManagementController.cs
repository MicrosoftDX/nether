// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Nether.Data.PlayerManagement;
using Nether.Web.Utilities;

//TO DO: The group and player Image type is not yet implemented. Seperate methods need to be implemented to upload a player or group image
//TODO: Add versioning support
//TODO: Add authentication


namespace Nether.Web.Features.PlayerManagement
{
    [Route("api")]
    public class PlayerManagementController : Controller
    {
        private readonly IPlayerManagementStore _store;

        public PlayerManagementController(IPlayerManagementStore store)
        {
            _store = store;
        }

        //Implementation of the player API

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

        [HttpGet("players/{playername}/")]
        public async Task<ActionResult> GetPlayer(string playername)
        {
            // Call data store
            var player = await _store.GetPlayerDetailsAsync(playername);

            if (player == null)
            {
                return NotFound();
            }

            // Format response model
            var resultModel = new PlayerGetResponseModel
            {
                Player = player
            };

            // Return result
            return Ok(resultModel);
        }



        // EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL
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
        // EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL EVIL



        [Authorize]
        [HttpGet("player")]
        public async Task<ActionResult> GetCurrentPlayer()
        {
            var playername = User.Identity.Name;

            // Call data store
            var player = await _store.GetPlayerDetailsByIdAsync(playername);

            if (player == null)
            {
                return NotFound();
            }

            // Format response model
            var resultModel = new PlayerGetResponseModel
            {
                Player = player
            };

            // Return result
            return Ok(resultModel);
        }
        [Authorize(Roles = "player")]
        [HttpGet("player/groups/")]
        public Task<ActionResult> GetPlayerGroups()
        {
            return GetPlayerGroups(User.GetGamerTag());
        }
        [HttpGet("players/{playername}/groups/")]
        public async Task<ActionResult> GetPlayerGroups(string playername)
        {
            // Call data store
            var groups = await _store.GetPlayersGroupsAsync(playername);

            // Format response model
            var resultModel = new GroupListResponseModel
            {
                Groups = groups.Cast<GroupListResponseModel.GroupsEntry>().ToList()
            };

            // Return result
            return Ok(resultModel);
        }

        [Authorize]
        [Route("players")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]PlayerPostRequestModel player)
        {
            //TODO: Handle exceptions and retries
            var gamerTag = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name) // For a quick implementation, assume that name is the gamertag - review later!
                ?.Value;
            if (string.IsNullOrWhiteSpace(gamerTag))
            {
                return BadRequest(); //TODO: return error info in body
            }

            // Save player
            await _store.SavePlayerAsync(new Player { Gamertag = gamerTag, Country = player.Country, CustomTag = player.CustomTag });

            // Return result
            var location = Url.Link("GetPlayer", new { playername = player.Gamertag });
            return Created("GetPlayer", new { playername = player.Gamertag });
        }

        [Authorize(Roles = "player")]
        [Route("players/{player}")]
        [HttpPut]
        public async Task<ActionResult> Put([FromBody]PlayerPostRequestModel player)
        {
            // Update player
            await _store.SavePlayerAsync(
                new Player { PlayerId = User.Identity.Name, Gamertag = player.Gamertag, Country = player.Country, CustomTag = player.CustomTag });

            // Return result
            return new NoContentResult();
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

        [HttpGet("groups")]
        public async Task<ActionResult> GetGroups()
        {
            // Call data store
            var groups = await _store.GetGroups();

            // Format response model
            var resultModel = new GroupListResponseModel
            {
                Groups = groups.Select(s => (GroupListResponseModel.GroupsEntry)s).ToList()
            };

            // Return result
            return Ok(resultModel);
        }

        [HttpGet("groups/{groupname}/", Name = "GetGroup")]
        public async Task<ActionResult> GetGroup(string groupname)
        {
            // Call data store
            var group = await _store.GetGroupDetailsAsync(groupname);
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
                Members = players.Cast<GroupMemberResponseModel.PlayersEntry>().ToList()
            };

            // Return result
            return Ok(resultModel);
        }

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
                    Players = group.Players
                }
            );

            // Return result
            var location = Url.Link("GetGroup", new { groupname = group.Name });
            return Created(location, null);
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


        [Route("groups/{group}")]
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
                        Players = group.Players
                    }
                );

            // Return result
            return new NoContentResult();
        }
    }
}
