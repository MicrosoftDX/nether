// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
