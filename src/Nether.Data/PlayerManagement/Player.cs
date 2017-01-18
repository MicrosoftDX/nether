// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace Nether.Data.PlayerManagement
{
    [DebuggerDisplay("Player (tag '{Gamertag}', UserId '{UserId'})")]
    public class Player
    {
        public string UserId { get; set; }
        public string Gamertag { get; set; }
        public string Country { get; set; }
        public string CustomTag { get; set; }
    }
}
