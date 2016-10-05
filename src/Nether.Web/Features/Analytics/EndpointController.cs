// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.AspNetCore.Mvc;
using Nether.Web.Utilities;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Nether.Web.Features.Analytics
{
    [Route("api/endpoint")]
    public class EndpointController : Controller
    {
        // GET: api/endpoint
        [HttpGet]
        public ActionResult Get()
        {
            //TODO: Pick up the hardcoded information below from configuration as soon as we have that implemented
            const string keyName = "game";
            const string sharedAccessKey = "rxvfLSc98g95pL6+iSAgDya88M+emLKgNXQ0+asRf/8=";
            const string resource = "https://nether.servicebus.windows.net/analytics/messages";
            var timeSpan = TimeSpan.FromHours(24);

            var validUntilUtc = DateTime.UtcNow + timeSpan;

            var authorization = SharedAccessSignatureTokenProviderEx.GetSharedAccessSignature(
                keyName,
                sharedAccessKey,
                resource,
                timeSpan);

            var result = new AnalyticsEndpointInfoResponseModel()
            {
                HttpVerb = "POST",
                Url = resource,
                ContentType = "application/json",
                Authorization = authorization,
                ValidUntilUtc = validUntilUtc
            };

            return Ok(result);
        }
    }
}

