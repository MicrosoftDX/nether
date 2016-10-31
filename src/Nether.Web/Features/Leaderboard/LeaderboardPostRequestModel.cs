// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Web.Features.Leaderboard
{
    public class LeaderboardPostRequestModel
    {
        public string Country { get; set; }
        public string CustomTag { get; set; }
        public int Score { get; set; }
    }
}
