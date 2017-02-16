// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using System;
using Nether.Data.PlayerManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Nether.Data.Sql.PlayerManagement
{
    [DebuggerDisplay("PlayerExtendedEntity (tag '{Gamertag}', UserId '{UserId'})")]
    public class PlayerExtendedEntity
    {
        public string UserId { get; set; }
        public string Gamertag { get; set; }
        public string ExtendedInformation { get; set; }
        public PlayerState ToPlayerExtended()
        {
            return new PlayerState
            {
                UserId = UserId,
                Gamertag = Gamertag,
                State = ExtendedInformation
            };
        }
    }
}
