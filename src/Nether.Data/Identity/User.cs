// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Identity
{
    [DebuggerDisplay("User ({UserId})")]
    public class User
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }

        public ICollection<Login> Logins { get; set; }
    }
}
