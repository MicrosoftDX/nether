// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nether.Data.Leaderboard;
using Microsoft.Extensions.Logging;
using System;

namespace Nether.Data.MongoDB.Leaderboard
{
    public class MongoDBLeaderboardStore : ILeaderboardStore
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDBLeaderboardStore> _logger;

        private IMongoCollection<MongoDBGameScore> ScoresCollection
            => _database.GetCollection<MongoDBGameScore>("scores");

        public MongoDBLeaderboardStore(string connectionString, string dbName, ILoggerFactory loggerFactory)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(dbName);
            _logger = loggerFactory.CreateLogger<MongoDBLeaderboardStore>();
        }


        public async Task SaveScoreAsync(GameScore gameScore)
        {
            _logger.LogDebug("Saving score {0} for gamertag '{1}", gameScore.Score, gameScore.Gamertag);
            await ScoresCollection.InsertOneAsync(gameScore);
        }


        public async Task<List<GameScore>> GetAllHighScoresAsync()
        {
            var query = from s in ScoresCollection.AsQueryable()
                        group s by s.Gamertag
                        into g
                        orderby g.Max(s => s.Score) descending
                        select new GameScore
                        {
                            Gamertag = g.Key,
                            Score = g.Max(s => s.Score)
                        };

            return await query.ToListAsync();
        }

        public async Task<List<GameScore>> GetTopHighScoresAsync(int n)
        {
            var query = (from s in ScoresCollection.AsQueryable()
                         group s by s.Gamertag
                        into g
                         orderby g.Max(s => s.Score) descending
                         select new GameScore
                         {
                             Gamertag = g.Key,
                             Score = g.Max(s => s.Score)
                         }).Take(n);

            return await query.ToListAsync();
        }

        public async Task<List<GameScore>> GetScoresAroundMe(int nBetter, int nWorse, string gamerTag)
        {
            var highScore = await GetHighScoreAsync(gamerTag);
            var gamerRank = await GetRankAsync(highScore.Score);

            var betterScores = (from s in ScoresCollection.AsQueryable()
                                where s.Score > highScore.Score
                                group s by s.Gamertag into g
                                orderby g.Max(s => s.Score)
                                select new GameScore
                                {
                                    Gamertag = g.Key,
                                    Score = g.Max(s => s.Score)
                                }).Take(nBetter);

            var lamerScores = (from s in ScoresCollection.AsQueryable()
                               where s.Score <= highScore.Score && s.Gamertag != gamerTag
                               group s by s.Gamertag into g
                               orderby g.Max(s => s.Score) descending
                               select new GameScore
                               {
                                   Gamertag = g.Key,
                                   Score = g.Max(s => s.Score)
                               }).Take(nWorse);

            var highScoreList = new List<GameScore>();

            //TODO: Inject rank into result here
            highScoreList.AddRange(await betterScores.ToListAsync());
            highScoreList.Reverse();
            highScoreList.Add(highScore);
            highScoreList.AddRange(await lamerScores.ToListAsync());

            return highScoreList;
        }

        private async Task<int> GetRankAsync(int score)
        {
            var getHigherScores = from s in ScoresCollection.AsQueryable()
                                  where s.Score > score
                                  select s;

            return await getHigherScores.CountAsync();
        }

        private async Task<GameScore> GetHighScoreAsync(string gamerTag)
        {
            var getGamerScores = from s in ScoresCollection.AsQueryable()
                                 where s.Gamertag == gamerTag
                                 orderby s.Score descending
                                 select new GameScore
                                 {
                                     Gamertag = s.Gamertag,
                                     Score = s.Score
                                 };

            return await getGamerScores.FirstOrDefaultAsync();
        }

        public Task<List<GameScore>> GetScoresAroundMe()
        {
            throw new NotImplementedException();
        }

        public Task<List<GameScore>> GetScoresAroundMe(string gamerTag, int radius)
        {
            throw new NotImplementedException();
        }
    }
}

