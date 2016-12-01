// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace Nether.Web.Features.Identity.Configuration
{
    public class Scopes
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            // TODO - review scope usage!
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "nether-all",
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "nether-all",
                            DisplayName = "Nether (All)",
                            Description = "All nether features",
                        }
                    },
                    UserClaims =
                    {
                        new UserClaim { Type = JwtClaimTypes.Subject },
                        new UserClaim { Type = JwtClaimTypes.Name },
                        new UserClaim { Type = JwtClaimTypes.NickName },
                        new UserClaim { Type = JwtClaimTypes.Role }
                    }
                }
            };
        }
    }
}
