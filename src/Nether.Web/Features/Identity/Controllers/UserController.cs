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
        private readonly FacebookGraphService _facebookGraphService;
        private readonly ILogger _logger;

        public UserController(IUserStore userStore,
            FacebookGraphService facebookGraphService,
            ILogger<UserController> logger)
        {
            _userStore = userStore;
            _facebookGraphService = facebookGraphService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves logins the currently logged in user.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserLoginListModel))]
        [HttpGet("logins")]
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
        [HttpGet("{providerType}")]
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
        [HttpDelete("logins/{providerType}", Name = nameof(DeleteUsersLogin))]
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
            var result = await _facebookGraphService.TokenDebugAsync(userLoginModel.FacebookToken);
            if (!result.IsValid || string.IsNullOrEmpty(result.UserId))
            {
                _logger.LogTrace("Facebook token validation failed: {0}", result.Error.Message);
                return this.ValidationFailed(new ErrorDetail("facebookToken", "Facebook token is either invalid or could not be validated."));
            }

            var providerId = result.UserId;
            var login = user.Logins.SingleOrDefault(l => l.ProviderType == providerType);
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
                login.ProviderId = providerId;
            }

            await _userStore.SaveUserAsync(user);

            //return CreatedAtRoute(nameof(DeleteUserLogin), new { providerType, providerId }, null);
            return CreatedAtRoute(nameof(GetLogin), new { providerType }, login);
        }


        private UserLoginModel MapLogin(Data.Identity.Login login)
        {
            return new UserLoginModel
            {
                ProviderType = login.ProviderType,
                ProviderId = login.ProviderId,
                _Link = Url.RouteUrl(nameof(GetLogin), new { providerType = login.ProviderType })
            };
        }
    }
}
