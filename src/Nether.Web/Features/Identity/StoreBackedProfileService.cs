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
using System.Net.Http;

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
            context.AddFilteredClaims(await GetUserClaimsAsync(user));
        }


        // TODO - Get URL from config!!
        private static HttpClient HttpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
        public static async Task<IEnumerable<Claim>> GetUserClaimsAsync(User user)
        {
            // TODO create claims for user (look up gamer tag, ...)

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserId),
                new Claim(JwtClaimTypes.Name, user.UserId),
                new Claim(JwtClaimTypes.Role, user.Role),
            };

            // TODO security...;-)
            var response = await HttpClient.GetAsync($"/api/EVIL/HELPER/tagfromid/{user.UserId}");
            if (response.IsSuccessStatusCode)
            {
                var gamertag = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(gamertag))
                {
                    claims.Add(new Claim(JwtClaimTypes.NickName, gamertag));
                }
            }
            return claims;
        }
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userStore.GetUserByIdAsync(context.Subject.GetSubjectId());

            context.IsActive = user?.IsActive ?? false;
        }
    }
}
