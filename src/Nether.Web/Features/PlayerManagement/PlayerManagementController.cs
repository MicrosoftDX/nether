// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;

using Nether.Data.PlayerManagement;
using System.Linq;
using Nether.Integration.Analytics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860


//TO DO: The group and player Image type is not yet implemented. Seperate methods need to be implemented to upload a player or group image
//TODO: Add versioning support
//TODO: Add authentication


namespace Nether.Web.Features.PlayerManagement
{
    [Route("api")]
    public class PlayerManagementController : Controller
    {
        private readonly IPlayerManagementStore _store;
        private readonly IAnalyticsIntegrationClient _analyticsIntegrationClient;

        public PlayerManagementController(IPlayerManagementStore store, IAnalyticsIntegrationClient analyticsIntegrationClient)
        {
            _store = store;
            _analyticsIntegrationClient = analyticsIntegrationClient;
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
                Players = players.Select(s => (PlayerListGetResponseModel.PlayersEntry)s).ToList()
            };

            // Return result
            return Ok(resultModel);
        }

        [HttpGet("players/{playername}/")]
        public async Task<ActionResult> GetPlayers(string playername)
        {
            // Call data store
            var player = await _store.GetPlayerDetailsAsync(playername);

            // Format response model
            var resultModel = new PlayerGetResponseModel
            {
                Player = player
            };

            // Return result
            return Ok(resultModel);
        }


        [HttpGet("players/{playername}/groups/")]
        public async Task<ActionResult> GetPlayerGroups(string playername)
        {
            // Call data store
            var groups = await _store.GetPlayersGroupsAsync(playername);

            // Format response model
            var resultModel = new GroupListResponseModel
            {
                Groups = groups.Select(s => (GroupListResponseModel.GroupsEntry)s).ToList()
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
                return StatusCode((int)HttpStatusCode.BadRequest); //TODO: return error info in body
            }

            // Save player
            await Task.WhenAll(
                _store.SavePlayerAsync(new Player { Gamertag = gamerTag, Country = player.Country, CustomTag = player.CustomTag }));

            // Return result
            return Ok();
        }

        [Route("players/{player}")]
        [HttpPut]
        public async Task<ActionResult> Put([FromBody]PlayerPostRequestModel player)
        {
            // Update player
            await Task.WhenAll(
                _store.SavePlayerAsync(new Player { Gamertag = player.Gamertag, Country = player.Country, CustomTag = player.CustomTag }));

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
            await Task.WhenAll(
                _store.AddPlayerToGroupAsync(group, player));


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

        [HttpGet("groups/{groupname}/")]
        public async Task<ActionResult> GetGroups(string groupname)
        {
            // Call data store
            var group = await _store.GetGroupDetailsAsync(groupname);

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

        [Route("groups")]
        [HttpPost]
        public async Task<ActionResult> PostGroup([FromBody]GroupPostRequestModel group)
        {
            // Save group
            await Task.WhenAll(
                _store.SaveGroupAsync(new Group { Name = group.Name, CustomType = group.CustomType, Description = group.Description, Players = group.Players }));

            // Return result
            return Ok();
        }

        [Route("groups/{groupname}/players/{playername}")]
        [HttpPost]
        public async Task<ActionResult> PostAddPlayerToGroup(string groupname, string playername)
        {
            Player player = await _store.GetPlayerDetailsAsync(playername);
            Group group = await _store.GetGroupDetailsAsync(groupname);

            // Save group
            await Task.WhenAll(
                _store.AddPlayerToGroupAsync(group, player));

            // Return result
            return Ok();
        }


        [Route("groups/{groupname}/players/{playername}")]
        [HttpDelete]
        public async Task<ActionResult> DeletePlayerFromGroup(string groupname, string playername)
        {
            Player player = await _store.GetPlayerDetailsAsync(playername);
            Group group = await _store.GetGroupDetailsAsync(groupname);

            await Task.WhenAll(
               _store.RemovePlayerFromGroupAsync(group, player));

            return new NoContentResult();
        }


        [Route("groups/{group}")]
        [HttpPut]
        public async Task<ActionResult> PutGroup([FromBody]GroupPostRequestModel group)
        {
            // Update group
            await Task.WhenAll(
                _store.SaveGroupAsync(new Group { Name = group.Name, CustomType = group.CustomType, Description = group.Description, Players = group.Players }));

            // Return result
            return new NoContentResult();
        }
    }
}
