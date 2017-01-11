// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

using IdentityServer4.Validation;

using Nether.Data.Identity;
using IdentityServer4.Models;

namespace Nether.Web.Features.Identity
{
    public class StoreBackedResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserStore _userStore;
        private readonly UserClaimsProvider _userClaimsProvider;
        private readonly IPasswordHasher _passwordHasher;

        public StoreBackedResourceOwnerPasswordValidator(
            IUserStore userStore,
            UserClaimsProvider userClaimsProvider,
            IPasswordHasher passwordHasher)
        {
            _userStore = userStore;
            _userClaimsProvider = userClaimsProvider;
            _passwordHasher = passwordHasher;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userStore.GetUserByUsernameAsync(context.UserName);
            if (user == null || !user.IsActive)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                return;
            }
            bool valid = _passwordHasher.VerifyHashedPassword(user.PasswordHash, context.Password);
            if (!valid)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                return;
            }

            var claims = await _userClaimsProvider.GetUserClaimsAsync(user);
            context.Result = new GrantValidationResult(user.UserId, "password", claims);
        }
    }
}
