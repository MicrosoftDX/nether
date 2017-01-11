// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nether.Data.Identity;
using Nether.Web.Features.Identity.Models;
using System.Linq;
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
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("user/{userid}")]
        public async Task<IActionResult> GetUser(string userid)
        {
            var user = await _userStore.GetUserByIdAsync(userid);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(UserResponseModel.MapFrom(user));
        }

    }
}
