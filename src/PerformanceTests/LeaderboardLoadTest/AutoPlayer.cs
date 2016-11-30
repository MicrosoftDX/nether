// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LeaderboardLoadTest
{
    public class AutoPlayer
    {
        private readonly TextWriter _logger;
        private readonly string _password;
        private readonly string _username;
        private readonly Random _random = new Random();
        private readonly string _playerInternalId = Guid.NewGuid().ToString();

        public AutoPlayer(string username, string password, TextWriter logger)
        {
            _username = username;
            _password = password;
            _logger = logger;
        }

        public async Task PlayGameAsync(CancellationToken cancellationToken)
        {
            int delayTime;
            var client = new NetherClient();
            var response = await client.LoginUserNamePasswordAsync(_username, _password);

            if (!response.IsSuccess)
            {
                _logger.WriteLine("Player({0}). Failed to log in, quitting: {1}", _playerInternalId, response.Message);
                return;
            }

            // simulate leaderboard activity
            while (!cancellationToken.IsCancellationRequested)
            {
                int count = _random.Next(1, 5);
                for (int i = 0; i < count; i++)
                {
                    int score = _random.Next(1500);

                    // send game score (POST)
                    _logger.WriteLine("Player({0}). Posting score {1}", _playerInternalId, score);
                    var postScoreResponse = await client.PostScoreAsync(score);
                    if (!postScoreResponse.IsSuccess)
                    {
                        _logger.WriteLine("Player({0}). Posting score failed: {1}", _playerInternalId, postScoreResponse.Message);
                    }
                    delayTime = _random.Next(1000, 10000);
                    await Task.Delay(delayTime);
                }
                
                // ask for leaderboard scores (GET)
                _logger.WriteLine("Player({0}). Getting all scores", _playerInternalId);
                var getScoresResponse = await client.GetScoresAsync();
                if (getScoresResponse.IsSuccess)
                {
                    _logger.WriteLine("Player({0}). Got scores {1}", _playerInternalId, getScoresResponse.Result);
                }
                else
                {
                    _logger.WriteLine("Player({0}). Failed to get scores: {1}", _playerInternalId, getScoresResponse.Message);
                }

                _logger.WriteLine("Player({0}). Getting top scores", _playerInternalId);
                var getTopScoresResponse = await client.GetScoresAsync("Top");
                if (getTopScoresResponse.IsSuccess)
                {
                    _logger.WriteLine("Player({0}). Got scores {1}", _playerInternalId, getTopScoresResponse.Result);
                }
                else
                {
                    _logger.WriteLine("Player({0}). Failed to get scores: {1}", _playerInternalId, getTopScoresResponse.Message);
                }                

                _logger.WriteLine("Player({0}). Getting palyers aroundme", _playerInternalId);
                var getAroundMeScoresResponse = await client.GetScoresAsync("AroundMe");
                if (getAroundMeScoresResponse.IsSuccess)
                {
                    _logger.WriteLine("Player({0}). Got scores {1}", _playerInternalId, getAroundMeScoresResponse.Result);
                }
                else
                {
                    _logger.WriteLine("Player({0}). Failed to get scores: {1}", _playerInternalId, getAroundMeScoresResponse.Message);
                }
                delayTime = _random.Next(1000, 10000);
                await Task.Delay(delayTime);
            }
        }
    }
}
