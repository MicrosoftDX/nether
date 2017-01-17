// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nether.Web.Features.PlayerManagement
{
    public class PlayerExtendedPutRequestModel
    {
        /// <summary>
        /// Gamertag, must be uniqueue.
        /// </summary>
        [Required]
        public string Gamertag { get; set; }

        /// <summary>
        /// Extended player information (e.g. JSON)
        /// </summary>
        public string ExtendedInformation { get; set; }
    }
}
