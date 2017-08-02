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
using Microsoft.Extensions.Options;

namespace Nether.Web.Features.Identity
{
    [Route("user/logins")]
    [NetherService("Identity")]
    [Authorize(Roles = RoleNames.Player)]
    public class UserLoginController : ControllerBase
    {
        private readonly IUserStore _userStore;
        private readonly FacebookGraphService _facebookGraphService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IOptions<SignInMethodOptions> _signInOptions;
        private readonly ILogger _logger;

        public UserLoginController(IUserStore userStore,
            FacebookGraphService facebookGraphService,
            IPasswordHasher passwordHasher,
            IOptions<SignInMethodOptions> signInOptions,
            ILogger<UserLoginController> logger)
        {
            _userStore = userStore;
            _facebookGraphService = facebookGraphService;
            _passwordHasher = passwordHasher;
            _signInOptions = signInOptions;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves logins the currently logged in user.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserLoginListModel))]
        [HttpGet("")]
        public async Task<IActionResult> GetLogins()
        {
            var user = await _userStore.GetCurrentUserAsync(User);

            var logins = user.Logins.Select(MapLogin);

            return Ok(new UserLoginListModel { Logins = logins });
        }

        /// <summary>
        /// Retrieves login for the specified prodiver for the currently logged in user.
        /// </summary>
        /// <param name="providerType">The provider type identifying the provider to be retrieved.</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserLoginModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("{providerType}", Name = nameof(UserLoginController) + "_" + nameof(GetLogin))]
        public async Task<IActionResult> GetLogin(string providerType)
        {
            var user = await _userStore.GetCurrentUserAsync(User);

            var login = user.Logins.SingleOrDefault(l => l.ProviderType == providerType);
            if (login == null)
            {
                return NotFound();
            }

            return Ok(MapLogin(login));
        }

        /// <summary>
        /// Deletes the user's login with the specified login details.
        /// </summary>
        /// <param name="providerType">The provider type identifying the provider to be removed.</param>
        /// <returns></returns>
        [HttpDelete("{providerType}")]
        public async Task<IActionResult> DeleteUsersLogin(string providerType)
        {
            var user = await _userStore.GetCurrentUserAsync(User);

            var login = user.Logins.SingleOrDefault(l => l.ProviderType == providerType);
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

        // TODO - in future I anticipate that this would be a single action
        //          with URL /logins/{providerType}
        //          and it would have a pluggable set of providers
        //          For now (in the interests of progress) it is implemented as separate actions!

        /// <summary>
        /// Update facebook login for the user
        /// </summary>
        /// <param name="userLoginModel">Any additional parameters required by the provider</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FacebookUserLoginRequestModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPut("facebook")]
        public async Task<IActionResult> PutFacebookUserLogin(
            [FromBody] FacebookUserLoginRequestModel userLoginModel)
        {
            const string providerType = "facebook";
            if (!_signInOptions.Value.Facebook.EnableAccessToken)
            {
                return this.ValidationFailed(new ErrorDetail("facebook", "Facebook access token sign-up is not allowed by configuration"));
            }
            var providerResult = await GetFacebookProviderLoginDetailsAsync(userLoginModel.FacebookToken);

            return await UpdateProviderLogin(providerType, providerResult);
        }

        /// <summary>
        /// Update username+password login for the user
        /// </summary>
        /// <param name="userLoginModel">Any additional parameters required by the provider</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UsernamePasswordUserLoginRequestModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPut("password")]
        public async Task<IActionResult> PutUsernamePasswordLogin(
            [FromBody] UsernamePasswordUserLoginRequestModel userLoginModel)
        {
            const string providerType = "password";
            if (!_signInOptions.Value.UsernamePassword.AllowUserSignUp)
            {
                return this.ValidationFailed(new ErrorDetail("password", "UsernamePassword sign-up is not allowed by configuration"));
            }
            var providerResult = await GetUsernamePasswordProviderLoginDetailsAsync(userLoginModel.Username, userLoginModel.Password);

            return await UpdateProviderLogin(providerType, providerResult);
        }

        private async Task<IActionResult> UpdateProviderLogin(string providerType, ProviderLoginDetails providerResult)
        {
            if (providerResult.ErrorDetail != null)
            {
                return this.ValidationFailed(providerResult.ErrorDetail);
            }

            var user = await _userStore.GetCurrentUserAsync(User);
            var login = user.Logins.SingleOrDefault(l => l.ProviderType == providerType);
            if (login == null)
            {
                login = new Login
                {
                    ProviderType = providerType,
                    ProviderId = providerResult.ProviderId,
                    ProviderData = providerResult.ProviderData
                };
                user.Logins.Add(login);
            }
            else
            {
                login.ProviderId = providerResult.ProviderId;
                login.ProviderData = providerResult.ProviderData;
            }

            await _userStore.SaveUserAsync(user);

            return CreatedAtRoute(nameof(UserLoginController) + "_" + nameof(GetLogin), new { providerType }, login);
        }

        // TODO - consider creating services that group the login provider specific implementations together
        // e.g. a service that can be injected into various places that handles all the facebook specific implementations across different controllers etc 
        private async Task<ProviderLoginDetails> GetFacebookProviderLoginDetailsAsync(string facebookToken)
        {
            // Validate the provided data
            if (string.IsNullOrEmpty(facebookToken))
            {
                _logger.LogInformation("Missing Facebook token.");
                return new ProviderLoginDetails
                {
                    ErrorDetail = new ErrorDetail("facebookToken", "Missing Facebook token.")
                };
            }

            // we need to verify the token first
            // since only Facebook is supported, we default to it, but in the future, this will likely move
            // to a provider-based pattern
            var result = await _facebookGraphService.TokenDebugAsync(facebookToken);
            if (!result.IsValid || string.IsNullOrEmpty(result.UserId))
            {
                _logger.LogTrace("Facebook token validation failed: {0}", result.Error.Message);
                return new ProviderLoginDetails
                {
                    ErrorDetail = new ErrorDetail("facebookToken", "Facebook token is either invalid or could not be validated.")
                };
            }
            return new ProviderLoginDetails
            {
                ProviderId = result.UserId,
                ProviderData = null
            };
        }
        private Task<ProviderLoginDetails> GetUsernamePasswordProviderLoginDetailsAsync(string username, string password)
        {
            // Validate the provided data
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogInformation("Missing username.");
                return Task.FromResult(new ProviderLoginDetails
                {
                    ErrorDetail = new ErrorDetail("password", "Missing username.")
                });
            }
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogInformation("Missing password");
                return Task.FromResult(new ProviderLoginDetails
                {
                    ErrorDetail = new ErrorDetail("password", "Missing password.")
                });
            }

            var providerData = _passwordHasher.HashPassword(password);

            return Task.FromResult(new ProviderLoginDetails
            {
                ProviderId = username,
                ProviderData = providerData
            });
        }
        private class ProviderLoginDetails
        {
            public ErrorDetail ErrorDetail { get; set; }
            public string ProviderId { get; set; }
            public string ProviderData { get; set; }
        }


        private UserLoginModel MapLogin(Data.Identity.Login login)
        {
            return new UserLoginModel
            {
                ProviderType = login.ProviderType,
                ProviderId = login.ProviderId,
                _Link = Url.RouteUrl(nameof(UserLoginController) + "_" + nameof(GetLogin), new { providerType = login.ProviderType })
            };
        }
    }
}
