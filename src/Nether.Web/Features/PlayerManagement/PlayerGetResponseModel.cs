// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement
{
    public class PlayerGetResponseModel
    {

        public PlayerEntry Player { get; set; }

        public class PlayerEntry
        {
            public static implicit operator PlayerEntry(Player player)
            {
                return new PlayerEntry { Gamertag = player.Gamertag, Country = player.Country, CustomTag = player.CustomTag, PlayerImage = player.PlayerImage };
            }

            public string Gamertag { get; set; }
            public string Country { get; set; }
            public string CustomTag { get; set; }

            // TO DO The Player Image get/set needs to be implemented
            public byte[] PlayerImage { get; set; }
        }
    }
}
