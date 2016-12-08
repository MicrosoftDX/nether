// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityServer4.Services.InMemory;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<FacebookUserAccessTokenExtensionGrantValidator> _logger;
        private readonly IUserStore _userStore;

        public FacebookUserAccessTokenExtensionGrantValidator(
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IUserStore userStore)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<FacebookUserAccessTokenExtensionGrantValidator>();
            _userStore = userStore;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            // TODO - add error handling/logging

            // TODO - refactor this code as there's a lot going on here!

            var appToken = _configuration["Identity:Facebook:AppToken"];

            var token = context.Request.Raw["token"];

            // Call facebook graph api to validate the token

            // TODO - reuse HttpClient
            var client = new HttpClient()
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

            var response = await client.GetAsync($"/debug_token?input_token={token}&access_token={appToken}");

            dynamic body = await response.Content.ReadAsAsync<dynamic>();


            if (body.data == null)
            {
                // Get here if (for example) the token is for a different application
                var message = (string)body.error.message;
                _logger.LogError("FacebookSignIn: error validating token: {0}", message);
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidRequest);
                return;
            }

            bool isValid = (bool)body.data.is_valid;
            var userId = (string)body.data.user_id;
            if (!isValid)
            {
                var message = (string)body.data.error.message;
                _logger.LogDebug("FacebookSignIn: invalid token: {0}", message);
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidRequest);
                return;
            }
            _logger.LogDebug("FacebookSignIn: Signing in: {0}", userId);

            // Look up the user
            // TODO - fix this so not hard-coded to in-memory user list, not thread-safe, ...
            var user = await _userStore.GetUserByFacebookIdAsync(userId);
            if (user == null)
            {
                user = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Role = "player",
                    IsActive = true,
                    FacebookUserId = userId
                };
                await _userStore.SaveUserAsync(user);
            }

            var claims = await StoreBackedProfileService.GetUserClaimsAsync(user); // TODO move this helper to somewhere more sensible
            context.Result = new GrantValidationResult(user.UserId, "nether-facebook", claims);

            return;
        }
    }
}
