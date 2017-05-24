// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Analytics.MessageFormats;

namespace AnalyticsTestClient
{
    internal class NetherAnalyticsNullClient : IAnalyticsClient
    {
        public Task SendMessageAsync(INetherMessage msg, DateTime? dbgEnqueuedTimeUtc = null)
        {
            Console.Write("_");
            return Task.CompletedTask;
        }
    }
}
