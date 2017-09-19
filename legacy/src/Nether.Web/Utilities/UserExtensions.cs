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
        /// <summary>
        /// Gets user gametag from current principal
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>Gametag or null if no gametag assigned</returns>
        public static string GetGamerTag(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.NickName)?.Value;
        }

        /// <summary>
        /// Gets user ID from current principal
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>User id</returns>
        public static string GetId(this ClaimsPrincipal user)
        {
            return user?.Identity.Name;
        }

        /// <summary>
        /// Gets the current user from the claims principal and claims store.
        /// </summary>
        /// <param name="userStore"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<Nether.Data.Identity.User> GetCurrentUserAsync(this Data.Identity.IUserStore userStore, ClaimsPrincipal user)
        {
            return await userStore.GetUserByIdAsync(user.GetId());
        }
    }
}
