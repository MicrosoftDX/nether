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
using Nether.Integration.Identity;
using Nether.Common.ApplicationPerformanceMonitoring;

namespace Nether.Web.Features.Identity
{
    public class StoreBackedProfileService : IProfileService
    {
        private readonly IUserStore _userStore;
        private readonly UserClaimsProvider _userClaimsProvider;
        private readonly ILogger _logger;
        private readonly IApplicationPerformanceMonitor _appMonitor;

        public StoreBackedProfileService(
            IUserStore userStore,
            UserClaimsProvider userClaimsProvider,
            IApplicationPerformanceMonitor appMonitor,
            ILogger<StoreBackedProfileService> logger)
        {
            _userStore = userStore;
            _userClaimsProvider = userClaimsProvider;
            _appMonitor = appMonitor;
            _logger = logger;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userStore.GetUserByIdAsync(context.Subject.GetSubjectId());

            context.IssuedClaims.Add(new Claim(JwtClaimTypes.Subject, user.UserId));
            context.AddFilteredClaims(await _userClaimsProvider.GetUserClaimsAsync(user));
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
