// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Web.Features.PlayerManagement.Models.Player
{
    public class PlayerGetResponseModel
    {
        public PlayerEntry Player { get; set; }

        public static PlayerGetResponseModel FromPlayer(Data.PlayerManagement.Player player)
        {
            return new PlayerGetResponseModel
            {
                Player = new PlayerEntry { Gamertag = player.Gamertag, Country = player.Country, CustomTag = player.CustomTag }
            };
        }

        public class PlayerEntry
        {
            /// <summary>
            /// Player gamertag
            /// </summary>
            public string Gamertag { get; set; }

            /// <summary>
            /// Country code
            /// </summary>
            public string Country { get; set; }

            /// <summary>
            /// Custom tag
            /// </summary>
            public string CustomTag { get; set; }
        }
    }
}
