// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;

namespace Nether.Web.Features.AdminWebUi
{
    [Route("admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AdminWebUiController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}