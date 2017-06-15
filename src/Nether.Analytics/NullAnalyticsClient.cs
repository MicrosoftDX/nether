// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics
{
    public class NullAnalyticsClient : IAnalyticsClient
    {
        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public Task SendMessageAsync(ITypeVersionMessage msg, DateTime? dbgEnqueuedTimeUtc = null)
        {
            return Task.CompletedTask;
        }

        public Task SendMessageAsync(string value)
        {
            return Task.CompletedTask;
        }
    }
}
