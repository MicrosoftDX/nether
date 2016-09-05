using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Identity.Development.Configuration
{
    public class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            // TODO - review scope usage!
            return new List<Scope>
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,

                new Scope
                {
                    Name = "nether-all",
                    DisplayName = "Nether (All)",
                    Description = "All nether features",
                    Type = ScopeType.Resource
                }
            };
        }
    }
}
