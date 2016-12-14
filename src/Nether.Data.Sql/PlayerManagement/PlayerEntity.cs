// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using System;
using Nether.Data.PlayerManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Nether.Data.Sql.PlayerManagement
{
    public class PlayerEntity
    {
        public Guid Id { get; set; }
        public string PlayerId { get; set; }
        public string Gamertag { get; set; }
        public string Country { get; set; }
        public string CustomTag { get; set; }

        public List<PlayerGroupEntity> PlayerGroups { get; set; }

        public Player ToPlayer()
        {
            return new Player
            {
                PlayerId = PlayerId,
                Gamertag = Gamertag,
                Country = Country,
                CustomTag = CustomTag
            };
        }
    }
}