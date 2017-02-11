// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement
{
    public class PlayerListGetResponseModel
    {
        public List<PlayersEntry> Players { get; set; }

        public class PlayersEntry
        {
            public static implicit operator PlayersEntry(Player player)
            {
                return new PlayersEntry { Gamertag = player.Gamertag, Country = player.Country, CustomTag = player.CustomTag };
            }

            public string Gamertag { get; set; }
            public string Country { get; set; }
            public string CustomTag { get; set; }

            // TO DO The Player Image get/set needs to be implemented
            public string imageUrl { get; set; }
        }
    }
}
