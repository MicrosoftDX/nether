// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading.Tasks;

namespace Nether.Integration.Analytics
{
    public interface IAnalyticsIntegrationClient
    {
        Task SendGameEventAsync(INetherMessage gameEvent);
    }

    public interface INetherMessage
    {

    }
}

