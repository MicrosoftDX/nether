// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Identity
{
    public class User
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }

        // username, password are only used for resource owner password login flow, so consider moving to "login" entity
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        // Going forward, FacebookUserId should be stored as a "login" for the "facebook" provider to allow generalisation to other identity types
        public string FacebookUserId { get; set; }
    }
}
