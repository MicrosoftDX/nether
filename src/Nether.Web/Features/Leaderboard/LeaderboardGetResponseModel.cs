// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Nether.Data.Leaderboard;

namespace Nether.Web.Features.Leaderboard
{
    public class LeaderboardGetResponseModel
    {
        public List<LeaderboardEntry> LeaderboardEntries { get; set; }

        public class LeaderboardEntry
        {
            public static implicit operator LeaderboardEntry(GameScore score)
            {
                return new LeaderboardEntry {Gamertag = score.Gamertag, Score = score.Score};
            }

            public string Gamertag { get; set; }
            public int Score { get; set; }
        }
    }
}
