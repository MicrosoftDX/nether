using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Integration.Identity
{
    /// <summary>
    /// Client used to allow the Identity feature to integrate with Player Management
    /// in a pluggable manner
    /// </summary>
    public interface IIdentityPlayerManagementClient
    {
        Task<string> GetGamertagForUserIdAsync(string userId);
    }
}
