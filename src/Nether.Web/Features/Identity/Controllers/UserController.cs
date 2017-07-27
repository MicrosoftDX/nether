// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nether.Data.Identity;
using Nether.Web.Features.Identity.Models.UserLogin;
using Nether.Web.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace Nether.Web.Features.Identity
{
    [Route("user")]
    [NetherService("Identity")]
    [Authorize(Roles = RoleNames.Player)]
    public class UserController : ControllerBase
    {
        private readonly IUserStore _userStore;
        private ILogger<UserController> _logger;

        public UserController(IUserStore userStore,
            IConfiguration configuration,
            ILogger<UserController> logger)
        {
            _userStore = userStore;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves the information of the currently logged in user.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserLoginListModel))]
        [HttpGet("logins")]
        public async Task<IActionResult> GetLogins()
        {
            var user = await _userStore.GetCurrentUserAsync(User);

            var logins = user.Logins.Select(l => new Models.UserLogin.UserLoginModel
            {
                ProviderType = l.ProviderType,
                ProviderId = l.ProviderId
            });
            return Ok(new UserLoginListModel { Logins = logins });
        }

        /// <summary>
        /// Deletes the user's login with the specified login details.
        /// </summary>
        /// <param name="providerId">The providerId to be deleted for the current user.</param>
        /// <param name="providerType">The provider type identifying the provider to be removed.</param>
        /// <returns></returns>
        [HttpDelete("logins/{providerType}/{providerId}", Name = nameof(DeleteUsersLogin))]
        public async Task<IActionResult> DeleteUsersLogin(string providerType, string providerId)
        {
            var user = await _userStore.GetCurrentUserAsync(User);

            var login = user.Logins.FirstOrDefault(l => l.ProviderType == providerType && l.ProviderId == providerId);
            if (login == null)
            {
                return NotFound();
            }

            if (user.Logins.Count == 1)
            {
                // we won't allow deleting *all* the login methods
                // as that would mean the user can't login anymore
                return this.ValidationFailed(new ErrorDetail("Logins", "Deleting the last login method is not allowed."));
            }

            user.Logins.Remove(login);

            await _userStore.SaveUserAsync(user);

            return NoContent();
        }

        /// <summary>
        /// Update the user and logins for the user
        /// </summary>
        /// <param name="providerType">The type of login provider</param>
        /// <param name="userLoginModel">Any additional parameters required by the provider</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserLoginRequestModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPost("logins/{providerType}", Name = nameof(PostUserLogin))]
        public async Task<IActionResult> PostUserLogin(
            [FromRoute] string providerType,
            [FromBody] UserLoginRequestModel userLoginModel)
        {
            var user = await _userStore.GetCurrentUserAsync(User);

            // Currently only support "facebook" provider
            if (!String.Equals(providerType, LoginProvider.FacebookUserAccessToken, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("PostUserLogin: Unsupported ProviderType '{0}'", providerType);
                return this.ValidationFailed(new ErrorDetail("providerType", "Unsupported provider type"));
            }

            // Validate the provided data
            if (string.IsNullOrEmpty(userLoginModel.FacebookToken))
            {
                _logger.LogInformation("PostUserLogin: Missing Facebook token.");
                return this.ValidationFailed(new ErrorDetail("facebookToken", "Missing Facebook token."));
            }

            // we need to verify the token first
            // since only Facebook is supported, we default to it, but in the future, this will likely move
            // to a provider-based pattern
            var providerId = await GetFacebookUserIdFromTokenAsync(userLoginModel.FacebookToken);

            if (string.IsNullOrEmpty(providerId))
            {
                return this.ValidationFailed(new ErrorDetail("facebookToken", "Facebook token is either invalid or could not be validated."));
            }

            user.Logins = user.Logins ?? new List<Login>();
            var login = user.Logins?.FirstOrDefault(l => l.ProviderType == providerType && l.ProviderId == providerId);
            if (login == null)
            {
                login = new Login
                {
                    ProviderType = providerType,
                    ProviderId = providerId,
                };
                user.Logins.Add(login);
            }
            else
            {
                // TODO: should we return a validation error here?
                return NoContent();
            }

            await _userStore.SaveUserAsync(user);

            //return CreatedAtRoute(nameof(DeleteUserLogin), new { providerType, providerId }, null);
            return CreatedAtRoute(nameof(PostUserLogin), login);
        }

        private IConfiguration _configuration;

        private async Task<string> GetFacebookUserIdFromTokenAsync(string token)
        {
            // Call facebook graph api to validate the token

            // As per https://developers.facebook.com/docs/facebook-login/access-tokens/#apptokens, we're using "appid|appsecret" for the app token
            var appId = _configuration["Identity:SignInMethods:Facebook:AppId"];
            var appSecret = _configuration["Identity:SignInMethods:Facebook:AppSecret"];
            var appToken = appId + "|" + appSecret;


            var client = CreateHttpClient();
            var response = await client.GetAsync($"/debug_token?input_token={token}&access_token={appToken}");

            dynamic body = await response.Content.ReadAsAsync<dynamic>();

            if (body.data == null)
            {
                // Get here if (for example) the token is for a different application
                var message = (string)body.error.message;
                _logger.LogError("FacebookSignIn: error validating token: {0}", message);
                return null;
            }

            bool isValid = (bool)body.data.is_valid;
            var userId = (string)body.data.user_id;
            if (!isValid)
            {
                var message = (string)body.data.error.message;
                _logger.LogError("FacebookSignIn: invalid token: {0}", message);
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
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                    }
                }
            };
        }


    }
}
