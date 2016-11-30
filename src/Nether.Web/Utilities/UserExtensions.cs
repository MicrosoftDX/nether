// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nether.Web.Utilities
{
    public static class UserExtensions
    {
        public static string GetGamerTag(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.NickName)?.Value;
        }
    }
}
