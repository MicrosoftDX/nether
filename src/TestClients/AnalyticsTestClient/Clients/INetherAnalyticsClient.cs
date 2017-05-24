// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Analytics.MessageFormats;
using System;
using System.Threading.Tasks;

namespace AnalyticsTestClient
{
    public interface IAnalyticsClient
    {
        Task SendMessageAsync(INetherMessage msg, DateTime? dbgEnqueuedTimeUtc = null);
    }
}
