// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Nether.Data.Sql.Identity
{
    [DebuggerDisplay("UserEntity ({UserId})")]
    public class UserEntity
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; } // TODO - allow multiple roles?

        public ICollection<LoginEntity> Logins { get; set; }
    }
}