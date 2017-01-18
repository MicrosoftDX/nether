// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Nether.Data.PlayerManagement
{
    [DebuggerDisplay("Group (name '{Name}')")]
    public class Group
    {
        public string Name { get; set; }

        public string CustomType { get; set; }

        public string Description { get; set; }

        public List<string> Members { get; set; }
    }
}
