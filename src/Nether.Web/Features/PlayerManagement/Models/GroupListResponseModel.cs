// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Nether.Data.PlayerManagement;
using System.Linq;

namespace Nether.Web.Features.PlayerManagement
{
    public class GroupListResponseModel
    {
        public List<GroupsEntry> Groups { get; set; }

        public class GroupsEntry
        {
            public string Name { get; set; }
            public string CustomType { get; set; }
            public string Description { get; set; }
        }

        public static GroupListResponseModel FromGroups(IEnumerable<Group> groups)
        {
            if (groups == null) return null;

            return new GroupListResponseModel
            {
                Groups = groups.Select(g => new GroupsEntry { Name = g.Name, CustomType = g.CustomType, Description = g.Description }).ToList()
            };
        }
    }
}
