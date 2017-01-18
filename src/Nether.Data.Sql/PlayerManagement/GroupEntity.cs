// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using System;
using Nether.Data.PlayerManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Nether.Data.Sql.PlayerManagement
{
    [DebuggerDisplay("GroupEntity (name '{Name}')")]
    public class GroupEntity
    {
        public string Name { get; set; }
        public string CustomType { get; set; }
        public string Description { get; set; }

        public byte[] Image { get; set; }

        public List<PlayerGroupEntity> PlayerGroups { get; set; }
    }
}