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
using Microsoft.Extensions.Logging;

namespace Nether.Web.Features.Identity
{
    public class StoreBackedProfileService : IProfileService
    {
        // TODO - Get URL from config!!
        private static HttpClient s_httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

        private readonly IUserStore _userStore;
        private readonly ILogger _logger;


        public StoreBackedProfileService(IUserStore userStore, ILoggerFactory loggerFactory)
        {
            _userStore = userStore;
            _logger = loggerFactory.CreateLogger<StoreBackedProfileService>();
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userStore.GetUserByIdAsync(context.Subject.GetSubjectId());

            context.IssuedClaims.Add(new Claim(JwtClaimTypes.Subject, user.UserId));
            context.AddFilteredClaims(await GetUserClaimsAsync(user));
        }

        public static async Task<IEnumerable<Claim>> GetUserClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserId),
                new Claim(JwtClaimTypes.Name, user.UserId),
                new Claim(JwtClaimTypes.Role, user.Role),
            };

            // TODO security...;-)
            var response = await s_httpClient.GetAsync($"/api/EVIL/HELPER/tagfromid/{user.UserId}");
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
            var userId = context.Subject.GetSubjectId();
            var user = await _userStore.GetUserByIdAsync(userId);

            if (user == null)
            {
                _logger.LogError("StoreBackedProfileService.IsActiveAsync - no user found for id '{0}'", userId);
                context.IsActive = false;
            }
            else
            {
                context.IsActive = user.IsActive;
            }
        }
    }
}
