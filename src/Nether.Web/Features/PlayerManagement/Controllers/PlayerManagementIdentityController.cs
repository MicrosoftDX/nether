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
using Nether.Web.Features.PlayerManagement.Models.PlayerManagementIdentity;
using Nether.Common.ApplicationPerformanceMonitoring;

//TODO: Add versioning support

namespace Nether.Web.Features.PlayerManagement
{
    /// <summary>
    /// Player management controller for Identity interactions
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Suppress this from Swagger etc as it's designed to serve internal needs currently
    [Authorize(Policy = PolicyName.NetherIdentityClientId)] // only allow this to be called from the 'nether_identity' client
    [NetherService("PlayerManagement")]
    public class PlayerManagementIdentityController : Controller
    {
        private readonly IPlayerManagementStore _store;
        private readonly IApplicationPerformanceMonitor _appMonitor;
        private readonly ILogger _logger;

        public PlayerManagementIdentityController(
            IPlayerManagementStore store,
            IApplicationPerformanceMonitor appMonitor,
            ILogger<PlayerManagementIdentityController> logger)
        {
            _appMonitor = appMonitor;
            _store = store;
            _logger = logger;
        }


        [HttpGet("playeridentity/player/{playerid}")]
        public async Task<ActionResult> GetGamertagFromPlayerId([FromRoute] string playerid)
        {
            // Call data store
            var player = await _store.GetPlayerDetailsByUserIdAsync(playerid);

            // Return result
            return Ok(new { gamertag = player?.Gamertag });
        }
        [HttpPost("playeridentity/player/{playerid}")]
        [ReturnValidationFailureOnInvalidModelState]
        public async Task<IActionResult> SetGamertagForPlayerId(
                [FromRoute] string playerid,
                [FromBody] SetGamertagRequestModel model)
        {
            var player = await _store.GetPlayerDetailsByUserIdAsync(playerid);
            if (player == null)
            {
                player = new Player { UserId = playerid };
            }
            if (!string.IsNullOrEmpty(player.Gamertag))
            {
                _logger.LogInformation("Player already has gamertag (cannot update) in SetGamertagForPlayerId");
                _appMonitor.LogEvent("RegisterFailed", properties: new Dictionary<string, string> {
                    { "Reason", "UserAlreadyHasGamertag" },
                    { "Source", "Interactive"}
                });
                return this.ValidationFailed(new ErrorDetail("gamertag", "Cannot update gamertag"));
            }

            player.Gamertag = model.Gamertag;
            await _store.SavePlayerAsync(player);

            _appMonitor.LogEvent("RegisterSucceeded", properties: new Dictionary<string, string> {
                { "Reason", "UserAlreadyHasGamertag" },
                { "Source", "Interactive"}
            });

            return Ok();
        }

        [HttpGet("playeridentity/gamertag/{gamertag}")]
        public async Task<ActionResult> TestGamerTag([FromRoute]string gamertag)
        {
            var player = await _store.GetPlayerDetailsByGamertagAsync(gamertag);
            return Ok(
                new TestGamerTagResponseModel
                {
                    Available = player == null
                });
        }
    }
}
