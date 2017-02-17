// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Nether.Data.Leaderboard;
using Nether.Integration.Analytics;
using Nether.Web.Utilities;
using System.Net;
using Microsoft.Extensions.Logging;
using Nether.Analytics.GameEvents;
using Nether.Web.Features.Leaderboard.Models.Score;

namespace Nether.Web.Features.Leaderboard
{
    /// <summary>
    /// Leaderboard management
    /// </summary>
    [Route("scores")]
    public class ScoreController : Controller
    {
        private readonly ILeaderboardStore _store;
        private readonly IAnalyticsIntegrationClient _analyticsIntegrationClient;
        private readonly ILogger<ScoreController> _logger;

        public ScoreController(
            ILeaderboardStore store,
            IAnalyticsIntegrationClient analyticsIntegrationClient,
            ILogger<ScoreController> logger
            )
        {
            _store = store;
            _analyticsIntegrationClient = analyticsIntegrationClient;
            _logger = logger;
        }


        /// <summary>
        /// Posts a new score of currently logged in player
        /// </summary>
        /// <param name="request">Achieved score, must be positive</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Roles = RoleNames.Player)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]ScorePostRequestModel request)
        {
            //TODO: Make validation more sophisticated, perhaps some games want/need negative scores
            // Validate input
            if (request.Score < 0)
            {
                _logger.LogError("score is negative ({0})", request.Score);
                return BadRequest(); //TODO: return error info in body
            }

            //TODO: Handle exceptions and retries
            var gamertag = User.GetGamerTag();
            if (string.IsNullOrWhiteSpace(gamertag))
            {
                _logger.LogError("user has no gamertag: '{0}'", User.GetId());
                return BadRequest(); //TODO: return error info in body
            }

            // Save score and call analytics in parallel
            await Task.WhenAll(
                _store.SaveScoreAsync(new GameScore
                {
                    Gamertag = gamertag,
                    Country = request.Country,
                    CustomTag = request.CustomTag,
                    Score = request.Score
                }),
                _analyticsIntegrationClient.SendGameEventAsync(new ScoreEvent()
                {
                    //GamerTag = gamertag,
                    ClientUtcTime = DateTime.UtcNow,
                    GameSessionId = "unknowngamesession",
                    Score = request.Score
                }));

            // Return result
            return Ok();
        }

        /// <summary>
        /// Deletes all score achievements for the logged in user
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Roles = RoleNames.Player)]
        [HttpDelete("")]
        public async Task<ActionResult> DropMyScores()
        {
            var gamerTag = User.GetGamerTag();
            if (string.IsNullOrWhiteSpace(gamerTag))
            {
                _logger.LogError("user has no gamertag: '{0}'", User.GetId());
                return BadRequest(); //TODO: return error info in body
            }

            await _store.DeleteAllScoresAsync(gamerTag);

            return Ok();
        }
    }
}

