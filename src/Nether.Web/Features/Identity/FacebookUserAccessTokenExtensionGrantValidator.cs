// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nether.Common.ApplicationPerformanceMonitoring;
using Nether.Data.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity
{
    public class FacebookUserAccessTokenExtensionGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => "fb-usertoken";

        private FacebookGraphService _facebookGraphService;
        private readonly UserClaimsProvider _userClaimsProvider;
        private readonly IUserStore _userStore;
        private readonly ILogger _logger;
        private readonly IApplicationPerformanceMonitor _appMonitor;

        public FacebookUserAccessTokenExtensionGrantValidator(
            FacebookGraphService facebookGraphService,
            IUserStore userStore,
            UserClaimsProvider userClaimsProvider,
            IApplicationPerformanceMonitor appMonitor,
            ILogger<FacebookUserAccessTokenExtensionGrantValidator> logger)
        {
            _facebookGraphService = facebookGraphService;
            _userClaimsProvider = userClaimsProvider;
            _userStore = userStore;
            _appMonitor = appMonitor;
            _logger = logger;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            try
            {
                var token = context.Request.Raw["token"];

                var facebookTokenDebug = await _facebookGraphService.TokenDebugAsync(token);
                if (!facebookTokenDebug.IsValid)
                {
                    var message = (string)facebookTokenDebug.Error.Message;
                    _logger.LogError("FacebookSignIn: invalid token: {0}", message);
                    _appMonitor.LogEvent("LoginFailed", $"FacebookSignIn: invalid token: {message}", new Dictionary<string, string> {
                        { "EventSubType", "InvalidToken" },
                        { "LoginType", "fb-usertoken" }
                    });
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                    return;
                }
                if (facebookTokenDebug.Error != null) // still got another error
                {
                    var message = (string)facebookTokenDebug.Error.Message;
                    _logger.LogError("FacebookSignIn: error validating token: {0}", message);
                    _appMonitor.LogEvent("LoginFailed", $"FacebookSignIn: error validating token: {message}", new Dictionary<string, string> {
                        { "EventSubType", "TokenValidationFailed" },
                        { "LoginType", "fb-usertoken" }
                    });
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                    return;
                }

                var userId = facebookTokenDebug.UserId;
                if (userId == null)
                {
                    return;
                }
                _logger.LogDebug("FacebookSignIn: Signing in: {0}", userId);

                User user = await GetOrCreateUserAsync(userId);

                var claims = await _userClaimsProvider.GetUserClaimsAsync(user);
                context.Result = new GrantValidationResult(user.UserId, "nether-facebook", claims);

                _appMonitor.LogEvent("LoginSucceeded", properties: new Dictionary<string, string> {
                        { "LoginType", "fb-usertoken" }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in ValidateAsync: {0}", ex);
                _appMonitor.LogError(ex, "Error in ValidateAsync", new Dictionary<string, string> {
                        { "EventType", "LoginFailed" },
                        { "EventSubType", "UnhandledException" },
                        { "LoginType", "fb-usertoken" }
                    });
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, ex.Message);
            }
        }

        private async Task<User> GetOrCreateUserAsync(string userId)
        {
            var user = await _userStore.GetUserByLoginAsync(LoginProvider.FacebookUserAccessToken, userId);
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
                                ProviderType = LoginProvider.FacebookUserAccessToken,
                                ProviderId = userId
                            }
                        }
                };
                await _userStore.SaveUserAsync(user);
            }

            return user;
        }
    }
}
