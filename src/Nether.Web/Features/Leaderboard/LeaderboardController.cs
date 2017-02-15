// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Nether.Data.Leaderboard;
using Nether.Integration.Analytics;
using Nether.Web.Features.Leaderboard.Configuration;
using Nether.Web.Utilities;
using System.Net;
using Microsoft.Extensions.Logging;
using Nether.Analytics.GameEvents;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Nether.Web.Features.Leaderboard
{
    /// <summary>
    /// Leaderboard management
    /// </summary>
    [Route("leaderboard")]
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardStore _store;
        private readonly IAnalyticsIntegrationClient _analyticsIntegrationClient;
        private readonly ILogger<LeaderboardController> _log;
        private readonly ILeaderboardConfiguration _configuration;

        public LeaderboardController(ILeaderboardStore store, IAnalyticsIntegrationClient analyticsIntegrationClient,
            ILogger<LeaderboardController> log, ILeaderboardConfiguration configuration)
        {
            _store = store;
            _analyticsIntegrationClient = analyticsIntegrationClient;
            _log = log;
            _configuration = configuration;
        }

        //TODO: Add versioning support

        /// <summary>
        /// Gets leaderboard by leaderboard configured name
        /// </summary>
        /// <param name="name">Name of the leaderboard</param>
        /// <returns>List of scores and gametags</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LeaderboardGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Authorize(Roles = RoleNames.PlayerOrAdmin)]
        [HttpGet("{name}")]
        public async Task<ActionResult> Get(string name)
        {
            //TODO
            var gamerTag = User.GetGamerTag();

            LeaderboardConfig config = _configuration.GetLeaderboardConfig(name);
            if (config == null)
            {
                return NotFound();
            }
            LeaderboardType type = config.Type;
            List<GameScore> scores;
            switch (type)
            {
                case LeaderboardType.AroundMe:
                    scores = await _store.GetScoresAroundMeAsync(gamerTag, config.Radius);
                    break;
                case LeaderboardType.Top:
                    scores = await _store.GetTopHighScoresAsync(config.Top);
                    break;
                default:
                    scores = await _store.GetAllHighScoresAsync();
                    break;
            }

            // Format response model
            var resultModel = new LeaderboardGetResponseModel
            {
                Entries = scores == null ? null : scores.Select(s => (LeaderboardGetResponseModel.LeaderboardEntry)s).ToList()
            };

            // Return result
            return Ok(resultModel);
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
        public async Task<ActionResult> Post([FromBody]LeaderboardPostRequestModel request)
        {
            //TODO: Make validation more sophisticated, perhaps some games want/need negative scores
            // Validate input
            if (request.Score < 0)
            {
                _log.LogError("score is negative ({0})", request.Score);
                return BadRequest(); //TODO: return error info in body
            }

            //TODO: Handle exceptions and retries
            var gamertag = User.GetGamerTag();
            if (string.IsNullOrWhiteSpace(gamertag))
            {
                _log.LogError("user has no gamertag: '{0}'", User.GetId());
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
                _log.LogError("user has no gamertag: '{0}'", User.GetId());
                return BadRequest(); //TODO: return error info in body
            }

            await _store.DeleteAllScoresAsync(gamerTag);

            return Ok();
        }
    }
}

