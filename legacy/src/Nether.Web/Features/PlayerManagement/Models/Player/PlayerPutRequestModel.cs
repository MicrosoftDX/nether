// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace Nether.Web.Features.PlayerManagement.Models.Player
{
    /// <summary>
    /// Player info request object
    /// </summary>
    public class PlayerPutRequestModel
    {
        /// <summary>
        /// Gamertag
        /// </summary>
        [Required]
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