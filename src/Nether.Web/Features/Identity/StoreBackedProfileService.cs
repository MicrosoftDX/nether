// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

using Nether.Data.Identity;

namespace Nether.Web.Features.Identity
{
    public class StoreBackedProfileService : IProfileService
    {
        private readonly IUserStore _userStore;

        public StoreBackedProfileService(IUserStore userStore)
        {
            _userStore = userStore;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userStore.GetUserByIdAsync(context.Subject.GetSubjectId());

            context.IssuedClaims.Add(new Claim(JwtClaimTypes.Subject, user.UserId));
            context.AddFilteredClaims(GetUserClaims(user));
        }

        public static IEnumerable<Claim> GetUserClaims(User user)
        {
            // TODO create claims for user (look up gamer tag, ...)
            yield return new Claim(ClaimTypes.Name, user.UserId);
            yield return new Claim(JwtClaimTypes.Name, user.UserId);
            yield return new Claim(JwtClaimTypes.Role, user.Role);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userStore.GetUserByIdAsync(context.Subject.GetSubjectId());

            context.IsActive = user?.IsActive ?? false;
        }
    }
}
