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
using Nether.Web.Features.Leaderboard.Models.Score;
using Nether.Common.ApplicationPerformanceMonitoring;
using System.Collections.Generic;

namespace Nether.Web.Features.Leaderboard
{
    /// <summary>
    /// Leaderboard management
    /// </summary>
    [Route("scores")]
    [NetherService("Leaderboard")]
    public class ScoreController : Controller
    {
        private readonly ILeaderboardStore _store;
        private readonly IAnalyticsIntegrationClient _analyticsIntegrationClient;
        private readonly IApplicationPerformanceMonitor _appMonitor;
        private readonly ILogger<ScoreController> _logger;

        public ScoreController(
            ILeaderboardStore store,
            IAnalyticsIntegrationClient analyticsIntegrationClient,
            IApplicationPerformanceMonitor appMonitor,
            ILogger<ScoreController> logger
            )
        {
            _store = store;
            _analyticsIntegrationClient = analyticsIntegrationClient;
            _appMonitor = appMonitor;
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
        public async Task<IActionResult> Post([FromBody]ScorePostRequestModel request)
        {
            //TODO: Make validation more sophisticated, perhaps some games want/need negative scores
            // Validate input
            if (request.Score < 0)
            {
                _logger.LogError("score is negative ({0})", request.Score);
                _appMonitor.LogEvent("InvalidScore", properties: new Dictionary<string, string> {
                        { "Score", request.Score.ToString() }
                    });
                return this.ValidationFailed(new ErrorDetail("score", "Score cannot be negative"));
            }

            //TODO: Handle exceptions and retries
            var gamertag = User.GetGamerTag();
            if (string.IsNullOrWhiteSpace(gamertag))
            {
                _logger.LogError("User has no gamertag: '{0}'", User.GetId());
                return this.ValidationFailed(new ErrorDetail("gamertag", "The user doesn't have a gamertag"));
            }


            var userId = User.GetId();

            // Save score and call analytics in parallel
            await Task.WhenAll(
                _store.SaveScoreAsync(new GameScore
                {
                    UserId = userId,
                    Country = request.Country,
                    Score = request.Score
                }),
                SendScoreEventAndLogErrors(request));

            _appMonitor.LogEvent("Score", properties: new Dictionary<string, string>{
                {"Score", request.Score.ToString()}
            });

            // Return result
            return Ok();
        }

        private Task SendScoreEventAndLogErrors(ScorePostRequestModel request)
        {
            //try
            //{
            //    await _analyticsIntegrationClient.SendGameEventAsync(new ScoreEvent()
            //    {
            //        //GamerTag = gamertag,
            //        ClientUtcTime = DateTime.UtcNow,
            //        GameSessionId = "unknowngamesession",
            //        Score = request.Score
            //    });
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError("Error sending analytics ScoreEvent: {0}", ex);
            //    _appMonitor.LogError(ex, properties: new Dictionary<string, string> {
            //        { "Score", request.Score.ToString() }
            //    });
            //}

            // Temporary disabling sending of Scores to Nether Analytics, since that message is currently not supported.
            // Will be enabled again once, we start supporting that message type again.

            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes all score achievements for the logged in user
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Roles = RoleNames.Player)]
        [HttpDelete("")]
        public async Task<IActionResult> DropMyScores()
        {
            var userId = User.GetId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("user has no user ID");
                return this.ValidationFailed(new ErrorDetail("userid", "The user doesn't have a userID"));
            }

            await _store.DeleteAllScoresAsync(userId);

            return Ok();
        }
    }
}

