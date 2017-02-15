// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Nether.Web.Features.Identity
{
    [Route("identity-test")]
    [Authorize]
    public class IdentityTestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            // Convert claim type, value pairs into a dictionary for easier consumption as JSON
            // Need to group as there can be multiple claims of the same type (e.g. 'scope')
            var result = User.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(
                    keySelector: g => g.Key,
                    elementSelector: g => g.Count() == 1 ? (object)g.First().Value : g.Select(t => t.Value).ToArray()
                );
            return new JsonResult(result);
        }
    }
}
