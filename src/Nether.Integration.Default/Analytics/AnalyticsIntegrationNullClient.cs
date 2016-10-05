// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Threading.Tasks;
using Nether.Integration.Analytics;

namespace Nether.Integration.Default.Analytics
{
    public class AnalyticsIntegrationNullClient : IAnalyticsIntegrationClient
    {
        public async Task SendGameEventAsync(GameEvent gameEvent)
        {
            await Task.CompletedTask;
        }
    }
}
