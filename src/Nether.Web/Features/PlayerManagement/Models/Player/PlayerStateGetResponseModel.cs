// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.PlayerManagement;
using Nether.Web.Utilities;
using Newtonsoft.Json;

namespace Nether.Web.Features.PlayerManagement.Models.Player
{
    public class PlayerStateGetResponseModel
    {
        public static PlayerStateGetResponseModel FromPlayer(PlayerState player)
        {
            return new PlayerStateGetResponseModel
            {
                Gamertag = player.Gamertag,
                State = player.State
            };
        }

        /// <summary>
        /// Player gamertag
        /// </summary>
        public string Gamertag { get; set; }

        /// <summary>
        /// State information
        /// </summary>
        [JsonConverter(typeof(StringAsJsonConverter))]
        public string State { get; set; }
    }
}
