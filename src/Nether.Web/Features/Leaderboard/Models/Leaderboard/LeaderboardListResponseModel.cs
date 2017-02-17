// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Nether.Data.Leaderboard;
using Newtonsoft.Json;

namespace Nether.Web.Features.Leaderboard.Models.Leaderboard
{
    public class LeaderboardListResponseModel
    {
        public List<LeaderboardSummaryModel> Leaderboards { get; set; }

        public class LeaderboardSummaryModel
        {
            public string Name { get; set; }

            [JsonProperty(PropertyName = "_link")]
            public string _Link { get; set; }
        }
    }
}