// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.PlayerManagement
{
    /// <summary>
    /// Player info request object
    /// </summary>
    public class PlayerPostRequestModel
    {
        /// <summary>
        /// Gamertag
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
