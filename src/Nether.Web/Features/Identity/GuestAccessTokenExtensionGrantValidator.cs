// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public string GrantType => "guest-access";

        public GuestAccessTokenExtensionGrantValidator(
            //IConfiguration configuration,
            IUserStore userStore,
            UserClaimsProvider userClaimsProvider,
            ILogger<GuestAccessTokenExtensionGrantValidator> logger)
        {
            //_configuration = configuration;
            _userStore = userStore;
            _userClaimsProvider = userClaimsProvider;
            _logger = logger;
        }
        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var guestToken = context.Request.Raw["token"];

            _logger.LogInformation("GuestAccessToken: Signing in with guest access token '{0}'", guestToken);

            return Task.FromResult(0);
        }

        private async Task<User> GetOrCreateUserAsync(string userId)
        {
            var user = await _userStore.GetUserByLoginAsync(LoginProvider.GuestAccess, userId);
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
                                ProviderId = userId
                            }
                        }
                };

                // TODO: user with guest access should probably have some maximum lifetime, etc.
                // TODO: Implement this
                //await _userStore.SaveUserAsync(user);
            }

            return user;
        }
    }
}
