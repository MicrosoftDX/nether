// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Nether.Web.Features.PlayerManagement.Models.GroupAdmin
{
    public class GroupGetResponseModel
    {
        public GroupEntry Group { get; set; }

        public class GroupEntry
        {
            public static implicit operator GroupEntry(Data.PlayerManagement.Group group)
            {
                return new GroupEntry { Name = group.Name, CustomType = group.CustomType, Description = group.Description, Members = group.Members };
            }

            public string Name { get; set; }
            public string CustomType { get; set; }
            public string Description { get; set; }
            public List<string> Members { get; set; }
        }
    }
}
