// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement
{
    public class GroupPostRequestModel
    {
        public string Name { get; set; }
        public string CustomType { get; set; }
        public string Description { get; set; }
       
        // TO DO The Group Image get/set needs to be implemented
        //public byte[] Image { get; set; }
        public List<Player> Players { get; set; }
    }
}
