using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Common.DependencyInjection
{
    public interface IDependencyFactory<T>
    {
        T CreateInstance(IServiceProvider services);
    }
}
