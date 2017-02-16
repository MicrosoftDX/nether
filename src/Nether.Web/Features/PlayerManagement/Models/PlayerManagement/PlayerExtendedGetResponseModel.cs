// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement.Models.PlayerManagement
{
    public class PlayerStateGetResponseModel
    {
        public PlayerExtendedEntry PlayerExtended { get; set; }

        public static PlayerStateGetResponseModel FromPlayer(PlayerState player)
        {
            return new PlayerStateGetResponseModel
            {
                PlayerExtended = new PlayerExtendedEntry { Gamertag = player.Gamertag, ExtendedInformation = player.State }
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
