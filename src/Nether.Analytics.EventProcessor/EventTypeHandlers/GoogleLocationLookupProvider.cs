// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Nether.Analytics.EventProcessor.EventTypeHandlers
{
    public class GoogleLocationLookupProvider : ILocationLookupProvider
    {
        Task<LocationLookupInfo> ILocationLookupProvider.Lookup(double lat, double lon)
        {
            throw new NotImplementedException();
        }
    }
}
