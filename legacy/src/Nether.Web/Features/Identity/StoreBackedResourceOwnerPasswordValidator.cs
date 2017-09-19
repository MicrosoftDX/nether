// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

using IdentityServer4.Validation;

using Nether.Data.Identity;
using IdentityServer4.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nether.Common.ApplicationPerformanceMonitoring;
using System.Collections.Generic;

namespace Nether.Web.Features.Identity
{
    public class StoreBackedResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserStore _userStore;
        private readonly UserClaimsProvider _userClaimsProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IApplicationPerformanceMonitor _appMonitor;
        private readonly ILogger<StoreBackedResourceOwnerPasswordValidator> _logger;

        public StoreBackedResourceOwnerPasswordValidator(
            IUserStore userStore,
            UserClaimsProvider userClaimsProvider,
            IPasswordHasher passwordHasher,
            IApplicationPerformanceMonitor appMonitor,
            ILogger<StoreBackedResourceOwnerPasswordValidator> logger)
        {
            _userStore = userStore;
            _userClaimsProvider = userClaimsProvider;
            _passwordHasher = passwordHasher;
            _appMonitor = appMonitor;
            _logger = logger;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var userName = context.UserName;
            var user = await _userStore.GetUserByLoginAsync(LoginProvider.UserNamePassword, userName);
            if (user == null)
            {
                _logger.LogTrace("User not found: '{0}'", userName);
                _appMonitor.LogEvent("LoginFailed", "ResourceOwner: user not found", new Dictionary<string, string> {
                        { "EventSubType", "UserNotFound" },
                        { "LoginType", "password" }
                    });
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                return;
            }
            if (!user.IsActive)
            {
                _logger.LogTrace("User inactive: '{0}'", userName);
                _appMonitor.LogEvent("LoginFailed", "ResourceOwner: user inactive", new Dictionary<string, string> {
                        { "EventSubType", "UserInactive" },
                        { "LoginType", "password" }
                    });
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                return;
            }


            var login = user.Logins.FirstOrDefault(l => l.ProviderType == LoginProvider.UserNamePassword);
            if (login == null)
            {
                // shouldn't get here!
                _logger.LogError("User does not have a username password login entry: '{0}'", userName);
                _appMonitor.LogEvent("LoginFailed", "ResourceOwner: user has no password login entry", new Dictionary<string, string> {
                        { "EventSubType", "UserHasNoPassword" },
                        { "LoginType", "password" }
                    });
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
            }


            bool valid = _passwordHasher.VerifyHashedPassword(
                hashedPassword: login.ProviderData,
                providedPassword: context.Password);
            if (!valid)
            {
                // TODO: usually, we'd want to log the number of attempts to prevent attacks
                _logger.LogError("User's password was not valid: '{0}'", userName);
                _appMonitor.LogEvent("LoginFailed", "ResourceOwner: username and password did not match", new Dictionary<string, string> {
                        { "EventSubType", "PasswordIncorrect" },
                        { "LoginType", "password" }
                    });
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                return;
            }


            var claims = await _userClaimsProvider.GetUserClaimsAsync(user);
            context.Result = new GrantValidationResult(user.UserId, "password", claims);
            _appMonitor.LogEvent("LoginSucceeded", properties: new Dictionary<string, string> {
                        { "LoginType", "password" }
                    });
        }
    }
}
