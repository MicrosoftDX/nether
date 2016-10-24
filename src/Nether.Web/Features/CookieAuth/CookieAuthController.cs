using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Nether.Web.Features.CookieAuth
{
    //
    // TODO !!!!!!!!!!!!!!!!!!!!!
    // This is a quick implementation of authentication using cookies
    // We should switch to Bearer Tokens. Auth0 has a nice write-up here: https://auth0.com/blog/cookies-vs-tokens-definitive-guide/
    // The token issuer middleware is no longer in ASP.NET Core, so we should look at Azure Active Directory B2C and Identity Server
    //

    [Route("cookie-auth")]
    public class CookieAuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CookieAuthController> _logger;

        public CookieAuthController(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration; // TODO - review config
            _logger = loggerFactory.CreateLogger<CookieAuthController>();
        }

        [HttpPost("sign-in/facebook")]
        public async Task<IActionResult> FacebookSignIn([FromBody] FacebookSignInModel model)
        {
            var appToken = _configuration["Facebook:AppToken"];

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

            var response = await client.GetAsync($"/debug_token?input_token={model.UserAccessToken}&access_token={appToken}");

            dynamic body = await response.Content.ReadAsAsync<dynamic>();

            if (body.data == null)
            {
                // Get here if (for example) the token is for a different application
                var message = (string)body.error.message;
                _logger.LogError("FacebookSignIn: error validating token: {0}", message);
                return BadRequest();
            }

            bool isValid = (bool)body.data.is_valid;
            var userId = (string)body.data.user_id;
            if (!isValid)
            {
                var message = (string)body.data.error.message;
                _logger.LogDebug("FacebookSignIn: invalid token: {0}", message);
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }
            _logger.LogDebug("FacebookSignIn: Signing in: {0}", userId);

            var scopes = ((JArray)body.data.scopes).Select(t => t.Value<string>());
            ClaimsPrincipal principal = new ClaimsPrincipal(
               new ClaimsIdentity(
                   new[]
                   {
                        new Claim(ClaimTypes.Name, $"fb-{userId}"), // TODO - need to figure out what user name should be. Gamertag? How to look up??
                        new Claim(ClaimTypes.NameIdentifier, userId),
                        new Claim(ClaimTypes.Role, "player"),
                   },
                   "nether-facebook"
               )
            );
            var authenticationProperties = new AuthenticationProperties()
            {
                Items =
                {
                    { "scopes", string.Join(",", scopes)},
                    { "userAccessToken", model.UserAccessToken }
                }
            };
            await HttpContext.Authentication.SignInAsync("NetherCookieAuth", principal, authenticationProperties);

            return Ok();
        }

        [Authorize]
        [HttpGet("sign-out")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.Authentication.SignOutAsync("NetherCookieAuth");
            return Ok();
        }



#if DEBUG
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        //               ____  _____ __  __  _____     _______   _____ _   _ ___ ____ _
        //              |  _ \| ____|  \/  |/ _ \ \   / / ____| |_   _| | | |_ _/ ___|| |
        //              | |_) |  _| | |\/| | | | \ \ / /|  _|     | | | |_| || |\___ \| |
        //              | _ < | |___| |  | | |_| |\ V / | |___    | | |  _  || | ___) |_|
        //              |_| \_\_____|_|  |_|\___/  \_/  |_____|   |_| |_| |_|___|____/(_)
        //
        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        [Authorize]
        [HttpGet("/dev-mode/show-info")]

        public async Task<IActionResult> DevModeShowInfo() //////////////////////////////////////TODO - REMOVE THIS!!! REMOVE THIS!!! REMOVE THIS!!! REMOVE THIS!!! REMOVE THIS!!! ////////////////////////
        {
            var info = await HttpContext.Authentication.GetAuthenticateInfoAsync("NetherCookieAuth");
            return Ok(
                new
                {
                    username = User.Identity.Name,
                    claims = User.Claims.ToDictionary(c => c.Type, c => c.Value),
                    properties = info.Properties.Items
                });
        }
#endif
    }
}
