// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Web.Features.Analytics
{
    public class AnalyticsEndpointInfoResponseModel
    {
        public string HttpVerb { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public string Authorization { get; set; }
        public DateTime ValidUntilUtc { get; set; }
    }
}

