// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

using IdentityServer4.Validation;

using Nether.Data.Identity;

namespace Nether.Web.Features.Identity
{
    public class StoreBackedResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserStore _userStore;
        private readonly IPasswordHasher _passwordHasher;

        public StoreBackedResourceOwnerPasswordValidator(IUserStore userStore, IPasswordHasher passwordHasher)
        {
            _userStore = userStore;
            _passwordHasher = passwordHasher;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userStore.GetUserByUsernameAsync(context.UserName);
            _passwordHasher.VerifyHashedPassword(user.PasswordHash, context.Password);

            if (user != null)
            {
                context.Result = new GrantValidationResult(user.UserId, "password", StoreBackedProfileService.GetUserClaims(user)); // TODO move this helper to somewhere more sensible
            }
        }
    }
}
