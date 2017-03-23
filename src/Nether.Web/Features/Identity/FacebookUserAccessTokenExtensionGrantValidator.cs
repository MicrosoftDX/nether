// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nether.Common.ApplicationPerformanceMonitoring;
using Nether.Data.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity
{
    public class FacebookUserAccessTokenExtensionGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => "fb-usertoken";

        private readonly IConfiguration _configuration;
        private readonly UserClaimsProvider _userClaimsProvider;
        private readonly IUserStore _userStore;
        private readonly ILogger _logger;
        private readonly IApplicationPerformanceMonitor _appMonitor;

        public FacebookUserAccessTokenExtensionGrantValidator(
            IConfiguration configuration,
            IUserStore userStore,
            UserClaimsProvider userClaimsProvider,
            IApplicationPerformanceMonitor appMonitor,
            ILogger<FacebookUserAccessTokenExtensionGrantValidator> logger)
        {
            _configuration = configuration;
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

                var userId = await GetFacebookUserIdFromTokenAsync(context, token);
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

        private static Lazy<HttpClient> s_httpClientLazy = new Lazy<HttpClient>(CreateHttpClient);

        private async Task<string> GetFacebookUserIdFromTokenAsync(ExtensionGrantValidationContext context, string token)
        {
            // Call facebook graph api to validate the token

            // As per https://developers.facebook.com/docs/facebook-login/access-tokens/#apptokens, we're using "appid|appsecret" for the app token
            var appId = _configuration["Identity:SignInMethods:Facebook:AppId"];
            var appSecret = _configuration["Identity:SignInMethods:Facebook:AppSecret"];
            var appToken = appId + "|" + appSecret;


            var client = s_httpClientLazy.Value;
            var response = await client.GetAsync($"/debug_token?input_token={token}&access_token={appToken}");

            dynamic body = await response.Content.ReadAsAsync<dynamic>();

            if (body.data == null)
            {
                // Get here if (for example) the token is for a different application
                var message = (string)body.error.message;
                _logger.LogError("FacebookSignIn: error validating token: {0}", message);
                _appMonitor.LogEvent("LoginFailed", $"FacebookSignIn: error validating token: {message}", new Dictionary<string, string> {
                        { "EventSubType", "TokenValidationFailed" },
                        { "LoginType", "fb-usertoken" }
                    });
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                return null;
            }

            bool isValid = (bool)body.data.is_valid;
            var userId = (string)body.data.user_id;
            if (!isValid)
            {
                var message = (string)body.data.error.message;
                _logger.LogError("FacebookSignIn: invalid token: {0}", message);
                _appMonitor.LogEvent("LoginFailed", $"FacebookSignIn: invalid token: {message}", new Dictionary<string, string> {
                        { "EventSubType", "InvalidToken" },
                        { "LoginType", "fb-usertoken" }
                    });
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                return null;
            }

            return userId;
        }

        private static HttpClient CreateHttpClient()
        {
            return new HttpClient()
            {
                BaseAddress = new Uri("https://graph.facebook.com/"),
                DefaultRequestHeaders =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/json")
                    }
                }
            };
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
