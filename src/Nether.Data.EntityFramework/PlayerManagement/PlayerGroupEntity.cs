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
    /// <summary>
    /// Maps a player to a group
    /// </summary>
    [DebuggerDisplay("PlayerGroupEntity (group '{GroupName}', tag '{Gamertag'})")]
    public class PlayerGroupEntity // see https://docs.microsoft.com/en-us/ef/core/modeling/relationships#many-to-many
    {
        public string GroupName { get; set; }
        public GroupEntity Group { get; set; }

        public string Gamertag { get; set; }
        public PlayerEntity Player { get; set; }
    }
}