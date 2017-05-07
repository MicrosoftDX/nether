// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Sql.Leaderboard
{
    public class QueriedGamerScore
    {
        public int Score { get; set; }
        public string Gamertag { get; set; }        
        public long Ranking { get; set; }
    }
}
