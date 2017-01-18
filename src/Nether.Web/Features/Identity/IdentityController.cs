// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nether.Data.Identity;
using Nether.Web.Features.Identity.Models;
using Swashbuckle.SwaggerGen.Annotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        private readonly IUserStore _userStore;

        public IdentityController(IUserStore userStore)
        {
            _userStore = userStore;
        }

        /// <summary>
        /// Return a list of users
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(UserListModel), "The list of users")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userStore.GetUsersAsync();

            var userSummaries = users.Select(u => new UserSummaryModel
            {
                UserId = u.UserId,
                Role = u.Role,
                _Link = Url.RouteUrl(nameof(GetUser), new { userId = u.UserId })
            });
            return Ok(new UserListModel { Users = userSummaries });
        }

        /// <summary>
        /// Get user and logins information for a user
        /// </summary>
        /// <param name="userId">The id of the user to retrieve</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(UserResponseModel), "The user details")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: "No such user id")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("users/{userId}", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser(string userId)
        {
            var user = await _userStore.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(UserResponseModel.MapFrom(user));
        }

        /// <summary>
        /// Update the user and logins for a user
        /// </summary>
        /// <param name="userId">The id of the user to update</param>
        /// <param name="userModel">The new user and logins details for the user</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(UserResponseModel), "Returns the updated details")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPut("users/{userId}")]
        public async Task<IActionResult> PutUser(string userId, UserRequestModel userModel)
        {
            var user = UserRequestModel.MapToUser(userModel, userId);
            await _userStore.SaveUserAsync(user);

            return Ok(UserResponseModel.MapFrom(user));
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="userModel">The new user and login details for the user (including user id)</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.Created, description: "The Location header contains the URL to GET the user details")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost("users")]
        public async Task<IActionResult> PostUser(UserRequestModel userModel)
        {
            var user = UserRequestModel.MapToUser(userModel, userId: null);
            await _userStore.SaveUserAsync(user);

            return CreatedAtRoute(nameof(GetUser), new { userId = user.UserId });
        }

        [SwaggerResponse((int)HttpStatusCode.OK, description: "The user has been deleted")]
        [Authorize(Roles = RoleNames.Admin)]
        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userStore.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            await _userStore.DeleteUserAsync(user.UserId);

            return Ok();
        }
    }
}
