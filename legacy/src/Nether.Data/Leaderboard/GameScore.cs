// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Nether.Data.Leaderboard
{
    [DebuggerDisplay("GameScore (tag '{UserId}', score {Score})")]
    public class GameScore
    {
        public string UserId { get; set; }
        public string Country { get; set; }
        public int Score { get; set; }
        public long Rank { get; set; }
    }
}

