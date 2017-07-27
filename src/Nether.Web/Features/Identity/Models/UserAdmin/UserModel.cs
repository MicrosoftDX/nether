// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity.Models.UserAdmin
{
    public class UserModel
    {
        public bool Active { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
        public List<UserLoginModel> Logins { get; set; }
    }
}
