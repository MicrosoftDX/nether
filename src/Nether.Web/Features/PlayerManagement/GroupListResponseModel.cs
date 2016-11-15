// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement
{
    public class GroupListResponseModel
    {
        public List<GroupsEntry> Groups { get; set; }

        public class GroupsEntry
        {
            public static implicit operator GroupsEntry(Group group)
            {
                return new GroupsEntry { Name = group.Name, CustomType = group.CustomType, Description = group.Description };
            }

            public string Name { get; set; }
            public string CustomType { get; set; }
            public string Description { get; set; }
        }
    }
}
