// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityServer4.Models;
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
        private readonly UserClaimsProvider _userClaimsProvider;
        private readonly IUserStore _userStore;
        private readonly ILogger _logger;

        public FacebookUserAccessTokenExtensionGrantValidator(
            IConfiguration configuration,
            IUserStore userStore,
            UserClaimsProvider userClaimsProvider,
            ILogger<FacebookUserAccessTokenExtensionGrantValidator> logger)
        {
            _configuration = configuration;
            _userClaimsProvider = userClaimsProvider;
            _userStore = userStore;
            _logger = logger;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            // TODO - add error handling/logging

            // TODO - refactor this code as there's a lot going on here!

            try
            {
                var appToken = _configuration["Identity:SignInMethods:FacebookUserAccessToken:AppToken"];

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
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                    return;
                }

                bool isValid = (bool)body.data.is_valid;
                var userId = (string)body.data.user_id;
                if (!isValid)
                {
                    var message = (string)body.data.error.message;
                    _logger.LogError("FacebookSignIn: invalid token: {0}", message);
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                    return;
                }
                _logger.LogDebug("FacebookSignIn: Signing in: {0}", userId);

                var user = await _userStore.GetUserByLoginAsync(LoginProvider.Facebook, userId);
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
                                ProviderType = LoginProvider.Facebook,
                                ProviderId = userId
                            }
                        }
                    };
                    await _userStore.SaveUserAsync(user);
                }

                var claims = await _userClaimsProvider.GetUserClaimsAsync(user);
                context.Result = new GrantValidationResult(user.UserId, "nether-facebook", claims);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in ValidateAsync: {0}", ex);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, ex.Message);
            }
        }
    }
}
