using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nether.Common.DependencyInjection
{
    /// <summary>
    /// Interface used to resolve the type that will perform service configuration
    /// </summary>
    public interface IDependencyConfiguration
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger);
    }
}
