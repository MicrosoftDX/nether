// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Nether.Ingest
{
    public interface IMessageRouter
    {
        Task RouteMessageAsync(string partitionId, Message msg);
        Task FlushAsync(string partitionId);
    }
}