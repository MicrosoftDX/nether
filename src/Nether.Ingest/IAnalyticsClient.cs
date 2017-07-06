// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Ingest
{
    public interface IAnalyticsClient
    {
        Task SendMessageAsync(ITypeVersionMessage msg, DateTime? dbgEnqueuedTimeUtc = null);
        Task SendMessageAsync(string value);
        Task FlushAsync();
    }
}
