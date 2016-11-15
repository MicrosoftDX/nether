// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement
{
    public class GroupMemberResponseModel
    {

        public List<PlayersEntry> Members { get; set; }

        public class PlayersEntry
        {

            public static implicit operator PlayersEntry(Player player)
            {
                return new PlayersEntry { Gamertag = player.Gamertag };
            }

            public string Gamertag { get; set; }


        }
    }
}
