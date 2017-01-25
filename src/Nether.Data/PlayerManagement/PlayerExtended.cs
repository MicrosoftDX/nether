// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Nether.Data.PlayerManagement
{
    [DebuggerDisplay("PlayerExtended (tag '{Gamertag}', UserId '{UserId}')")]
    public class PlayerExtended
    {
        public string UserId { get; set; }
        public string Gamertag { get; set; }
        public string ExtendedInformation { get; set; }
    }
}
