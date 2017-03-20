using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nether.Common.DependencyInjection
{
    public interface IDependencyInitializer<T>
    {
        IApplicationBuilder Use(IApplicationBuilder app);
    }
}
