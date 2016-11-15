// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.PlayerManagement
{
    public class PlayerPostRequestModel
    {
        public string Gamertag { get; set; }
        public string Country { get; set; }
        public string CustomTag { get; set; }
    }
}
