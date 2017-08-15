// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nether.Common.ApplicationPerformanceMonitoring;
using Nether.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity
{
    public class GuestAccessTokenExtensionGrantValidator : IExtensionGrantValidator
    {
        private IUserStore _userStore;
        private UserClaimsProvider _userClaimsProvider;
        private ILogger<GuestAccessTokenExtensionGrantValidator> _logger;
        private IApplicationPerformanceMonitor _appMonitor;

        public string GrantType => "guest-access";

        public GuestAccessTokenExtensionGrantValidator(
            //IConfiguration configuration,
            IUserStore userStore,
            UserClaimsProvider userClaimsProvider,
            IApplicationPerformanceMonitor appMonitor,
            ILogger<GuestAccessTokenExtensionGrantValidator> logger)
        {
            //_configuration = configuration;
            _userStore = userStore;
            _userClaimsProvider = userClaimsProvider;
            _logger = logger;
            _appMonitor = appMonitor;
        }
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var guestIdentifier = context.Request.Raw["guest_identifier"];

            _logger.LogInformation("GuestAccessToken: Signing in with guest access token '{0}'", guestIdentifier);

            User user = await GetOrCreateGuestUserAsync(guestIdentifier);

            var claims = await _userClaimsProvider.GetUserClaimsAsync(user);
            context.Result = new GrantValidationResult(user.UserId, "nether-guest", claims);

            _appMonitor.LogEvent("LoginSucceeded", properties: new Dictionary<string, string> {
                        { "LoginType", "guest-access" }
                    });
        }

        private async Task<User> GetOrCreateGuestUserAsync(string guestIdentifier)
        {
            var user = await _userStore.GetUserByLoginAsync(LoginProvider.GuestAccess, guestIdentifier);
            if (user == null)
            {
                user = new User
                {
                    Role = RoleNames.Player,
                    IsActive = true,
                    Logins = new List<Login>
                        {
                            new Login
                            {
                                ProviderType = LoginProvider.GuestAccess,
                                ProviderId = guestIdentifier
                            }
                        }
                };

                // TODO: user with guest access should probably have some maximum lifetime, etc.
                await _userStore.SaveUserAsync(user);
            }

            return user;
        }
    }
}
