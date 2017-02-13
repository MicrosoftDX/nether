// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Nether.Data.Identity;
using IdentityServer4.Validation;
using IdentityServer4;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Http.Authentication;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nether.Integration.Identity;

namespace Nether.Web.Features.IdentityUi
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    public class AccountController : Controller
    {
        private readonly IUserStore _userStore;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly AccountService _account;
        private readonly IResourceOwnerPasswordValidator _passwordValidator;
        private readonly IIdentityPlayerManagementClient _playerManagementClient;
        private readonly ILogger _logger;

        public AccountController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IHttpContextAccessor httpContextAccessor,
            IUserStore userStore,
            IResourceOwnerPasswordValidator passwordValidator,
            IIdentityPlayerManagementClient playerManagementClient,
            ILogger<AccountController> logger)
        {
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            _userStore = userStore;
            _passwordValidator = passwordValidator;
            _interaction = interaction;
            _account = new AccountService(interaction, httpContextAccessor, clientStore);
            _playerManagementClient = playerManagementClient;
            _logger = logger;
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // clear external cookie in case it's left lingering, otherwise we get AccessDenied redirect: https://github.com/aspnet/Templates/issues/686
            await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            var vm = await _account.BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly && vm.ExternalProviders.Count() == 1)
            {
                // only one option for logging in
                return ExternalLogin(vm.ExternalProviders.First().AuthenticationScheme, returnUrl);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                // validate username/password against in-memory store
                var validationContext = new ResourceOwnerPasswordValidationContext { UserName = model.Username, Password = model.Password };
                await _passwordValidator.ValidateAsync(validationContext);
                if (!validationContext.Result.IsError)
                {
                    AuthenticationProperties props = null;
                    // only set explicit expiration here if persistent. 
                    // otherwise we reply upon expiration configured in cookie middleware.
                    if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                        };
                    };

                    // issue authentication cookie with subject ID and username
                    var user = await _userStore.GetUserByLoginAsync(LoginProvider.UserNamePassword, model.Username);
                    var userId = user.UserId;
                    await HttpContext.Authentication.SignInAsync(userId, model.Username, props);

                    // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await _account.BuildLoginViewModelAsync(model);
            return View(vm);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = await _account.BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // no need to show prompt
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            var vm = await _account.BuildLoggedOutViewModelAsync(model.LogoutId);
            if (vm.TriggerExternalSignout)
            {
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });
                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.Authentication.SignOutAsync(vm.ExternalAuthenticationScheme,
                        new AuthenticationProperties { RedirectUri = url });
                }
                catch (NotSupportedException) // this is for the external providers that don't have signout
                {
                }
                catch (InvalidOperationException) // this is for Windows/Negotiate
                {
                }
            }

            // delete local authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            return View("LoggedOut", vm);
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            returnUrl = Url.Action("ExternalLoginCallback", new { returnUrl = returnUrl });

            // start challenge and roundtrip the return URL
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                Items = { { "scheme", provider } }
            };
            return new ChallengeResult(provider, props);
        }

        ///// <summary>
        ///// Post processing of external authentication
        ///// </summary>
        //[HttpGet]
        //public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    // read external identity from the temporary cookie
        //    var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        //    var tempUser = info?.Principal;
        //    if (tempUser == null)
        //    {
        //        throw new Exception("External authentication error");
        //    }

        //    // retrieve claims of the external user
        //    var claims = tempUser.Claims.ToList();

        //    // try to determine the unique id of the external user - the most common claim type for that are the sub claim and the NameIdentifier
        //    // depending on the external provider, some other claim type might be used
        //    var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
        //    if (userIdClaim == null)
        //    {
        //        userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        //    }
        //    if (userIdClaim == null)
        //    {
        //        throw new Exception("Unknown userid");
        //    }

        //    // remove the user id claim from the claims collection and move to the userId property
        //    // also set the name of the external authentication provider
        //    claims.Remove(userIdClaim);
        //    var providerType = info.Properties.Items["scheme"];
        //    var providerUserId = userIdClaim.Value;

        //    // check if the external user is already provisioned
        //    var user = await _userStore.GetUserByLoginAsync(providerType, providerUserId);
        //    if (user == null)
        //    {
        //        // this sample simply auto-provisions new external user
        //        // another common approach is to start a registrations workflow first
        //        //user = _userStore.AutoProvisionUser(providerId, userId, claims);
        //        user = new User
        //        {
        //            Role = RoleNames.Player,
        //            IsActive = true,
        //            Logins  = new [] {
        //                new Login
        //                {
        //                    ProviderType = providerType,
        //                    ProviderId = providerUserId
        //                }
        //            }
        //        };
        //        _logger.LogInformation("Creating user from external source '{0}'", providerType);
        //        await _userStore.SaveUserAsync(user);
        //    }
        //    // TODO check for a gamertag and direct to "registration" page if not
        //    // Need to think about re-auth flow to pick up the tag!! (or can we keep the temp cookie until then?)
        //    throw new NotImplementedException("TODO - check and provision user with gamertag!");

        //    var additionalClaims = new List<Claim>();

        //    // if the external system sent a session id claim, copy it over
        //    var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        //    if (sid != null)
        //    {
        //        additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        //    }

        //    // if the external provider issued an id_token, we'll keep it for signout
        //    AuthenticationProperties props = null;
        //    var id_token = info.Properties.GetTokenValue("id_token");
        //    if (id_token != null)
        //    {
        //        props = new AuthenticationProperties();
        //        props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
        //    }

        //    // issue authentication cookie for user
        //    await HttpContext.Authentication.SignInAsync(user.UserId, "TODO - username!!", providerType, props, additionalClaims.ToArray());

        //    // delete temporary cookie used during external authentication
        //    await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        //    // validate return URL and redirect back to authorization endpoint
        //    if (_interaction.IsValidReturnUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }

        //    return Redirect("~/");
        //}

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            // read external identity from the temporary cookie
            var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new Exception("External authentication error");
            }

            // retrieve claims of the external user
            var claims = tempUser.Claims.ToList();
            Claim userIdClaim = GetUserIdClaim(claims);

            // remove the user id claim from the claims collection and move to the userId property
            // also set the name of the external authentication provider
            claims.Remove(userIdClaim);
            var providerType = info.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // check if the external user is already provisioned
            var user = await _userStore.GetUserByLoginAsync(providerType, providerUserId);
            if (user == null)
            {
                // this sample simply auto-provisions new external user
                // another common approach is to start a registrations workflow first
                //user = _userStore.AutoProvisionUser(providerId, userId, claims);
                user = new User
                {
                    Role = RoleNames.Player,
                    IsActive = true,
                    Logins = new[] {
                        new Login
                        {
                            ProviderType = providerType,
                            ProviderId = providerUserId
                        }
                    }
                };
                _logger.LogInformation("Creating user from external source '{0}'", providerType);
                await _userStore.SaveUserAsync(user);
            }
            // Check for a gamertag and render registration page if not
            var gamertag = await _playerManagementClient.GetGamertagForUserIdAsync(user.UserId);
            if (string.IsNullOrEmpty(gamertag))
            {
                return View("Register", new RegisterViewModel
                {
                    ReturnUrl = returnUrl
                });
            }

            return await SwitchToNetherAuthAndRedirectAsync(returnUrl, info, claims, providerType, user, gamertag);
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            // TODO - a lot of code between this and ExternalLoginCallback is shared - look at ways to refactor!

            // read external identity from the temporary cookie
            var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new Exception("External authentication error");
            }

            // retrieve claims of the external user
            var claims = tempUser.Claims.ToList();
            var userIdClaim = GetUserIdClaim(claims);

            // remove the user id claim from the claims collection and move to the userId property
            // also set the name of the external authentication provider
            claims.Remove(userIdClaim);
            var providerType = info.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // check if the external user is already provisioned
            var user = await _userStore.GetUserByLoginAsync(providerType, providerUserId);
            if (user == null)
            {
                // User should have been created in ExternalLoginCallback before getting here
                _logger.LogError("User does not exist in user store (in Register action)");
                throw new Exception("User should exist in Register action");
            }

            await _playerManagementClient.SetGamertagforUserIdAsync(user.UserId, model.Gamertag);

            return await SwitchToNetherAuthAndRedirectAsync(model.ReturnUrl, info, claims, providerType, user, model.Gamertag);
        }
        private async Task<IActionResult> SwitchToNetherAuthAndRedirectAsync(string returnUrl, AuthenticateInfo info, List<Claim> claims, string providerType, User user, string gamertag)
        {
            var additionalClaims = new List<Claim>();

            // if the external system sent a session id claim, copy it over
            var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            AuthenticationProperties props = null;
            var id_token = info.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                props = new AuthenticationProperties();
                props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }

            // issue authentication cookie for user
            await HttpContext.Authentication.SignInAsync(user.UserId, gamertag, providerType, props, additionalClaims.ToArray());

            // delete temporary cookie used during external authentication
            await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // validate return URL and redirect back to authorization endpoint
            if (_interaction.IsValidReturnUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        private static Claim GetUserIdClaim(List<Claim> claims)
        {
            // try to determine the unique id of the external user - the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            return userIdClaim;
        }
    }
}