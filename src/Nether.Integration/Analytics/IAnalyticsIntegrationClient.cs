// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Nether.Analytics.GameEvents;

namespace Nether.Integration.Analytics
{
    public interface IAnalyticsIntegrationClient
    {
        Task SendGameEventAsync(IGameEvent gameEvent);
    }


}

