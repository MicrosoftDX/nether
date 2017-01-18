// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement
{
    public class PlayerExtendedGetResponseModel
    {
        public PlayerExtendedEntry PlayerExtended { get; set; }

        public static PlayerExtendedGetResponseModel FromPlayer(PlayerExtended player)
        {
            return new PlayerExtendedGetResponseModel
            {
                PlayerExtended = new PlayerExtendedEntry { Gamertag = player.Gamertag, ExtendedInformation = player.ExtendedInformation }
            };
        }

        public class PlayerExtendedEntry
        {
            /// <summary>
            /// Player gamertag
            /// </summary>
            public string Gamertag { get; set; }

            /// <summary>
            /// Extended Information
            /// </summary>
            public string ExtendedInformation { get; set; }
        }
    }
}
