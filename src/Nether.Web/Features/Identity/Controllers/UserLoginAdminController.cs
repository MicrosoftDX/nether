// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nether.Data.Identity;
using Nether.Web.Features.Identity.Models.UserLoginAdmin;
using Nether.Web.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity
{
    [Route("admin/users/{userId}/logins")]
    [Authorize(Roles = RoleNames.Admin)]
    [NetherService("Identity")]
    public class UserLoginAdminController : ControllerBase
    {
        private readonly IUserStore _userStore;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger _logger;

        public UserLoginAdminController(
            IUserStore userStore,
            IPasswordHasher passwordHasher,
            ILogger<UserLoginAdminController> logger
            )
        {
            _userStore = userStore;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        /// <summary>
        /// Return a list of logins for the specified user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserLoginListModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet()]
        public async Task<IActionResult> GetAllLoginsForUser(string userId)
        {
            var user = await _userStore.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var logins = user.Logins.Select(l => new UserLoginModel
            {
                ProviderType = l.ProviderType,
                ProviderId = l.ProviderId,
                _Link = Url.RouteUrl(nameof(DeleteUserLogin), new { providerType = l.ProviderType, providerId = l.ProviderId })
            });
            return Ok(new UserLoginListModel { Logins = logins });
        }

        /// <summary>
        /// Update the user and logins for a user
        /// </summary>
        /// <param name="userId">The id of the user to update</param>
        /// <param name="providerType">The type of login provider</param>
        /// <param name="providerId">The provider specific id for the login</param>
        /// <param name="userLoginModel">Any additional parameters required by the provider</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserLoginRequestModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPut("{providerType}/{providerId}")]
        public async Task<IActionResult> PutUserLogin(
            [FromRoute] string userId,
            [FromRoute] string providerType,
            [FromRoute] string providerId,
            [FromBody] UserLoginRequestModel userLoginModel)
        {
            // Currently only support "password" provider
            providerType = providerType.ToLowerInvariant();
            if (providerType != LoginProvider.UserNamePassword)
            {
                _logger.LogInformation("PutUserLogin: Unsupported ProviderType '{0}'", providerType);
                return this.ValidationFailed(new ErrorDetail("providerType", "Unsupported provider type"));
            }

            // TODO Add password complexity options!
            var user = await _userStore.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
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
            // Set Password
            login.ProviderData = _passwordHasher.HashPassword(userLoginModel.Password);

            await _userStore.SaveUserAsync(user);

            return CreatedAtRoute(nameof(DeleteUserLogin), new { userId, providerType, providerId }, null);
        }

        /// <summary>
        /// Deletes the specified login for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="providerType"></param>
        /// <param name="providerId"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpDelete("{providerType}/{providerId}", Name = nameof(DeleteUserLogin))]
        public async Task<IActionResult> DeleteUserLogin([FromRoute] string userId, [FromRoute] string providerType, [FromRoute] string providerId)
        {
            var user = await _userStore.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var login = user.Logins.FirstOrDefault(l => l.ProviderType == providerType && l.ProviderId == providerId);
            if (login == null)
            {
                return NotFound();
            }

            user.Logins.Remove(login);

            await _userStore.SaveUserAsync(user);

            return NoContent();
        }
    }
}
