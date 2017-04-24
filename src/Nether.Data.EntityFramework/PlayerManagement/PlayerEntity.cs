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

namespace Nether.Data.EntityFramework.PlayerManagement
{
    [DebuggerDisplay("PlayerEntity (tag '{Gamertag}', UserId '{UserId}')")]
    public class PlayerEntity
    {
        public string UserId { get; set; }
        public string Gamertag { get; set; }
        public string Country { get; set; }
        public string CustomTag { get; set; }

        public Player ToPlayer()
        {
            return new Player
            {
                UserId = UserId,
                Gamertag = Gamertag,
                Country = Country,
                CustomTag = CustomTag
            };
        }
    }
}