// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.AspNetCore.Mvc;
using Nether.Web.Utilities;
using Microsoft.Extensions.Configuration;
using System.Net;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Nether.Web.Features.Analytics
{
    [Route("api/endpoint")]
    public class EndpointController : Controller
    {
        private readonly EndpointInfo _endpointInfo;

        public EndpointController(EndpointInfo endpointInfo)
        {
            _endpointInfo = endpointInfo;
        }
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AnalyticsEndpointInfoResponseModel))]
        [HttpGet]
        public IActionResult Get()
        {
            Console.WriteLine();
            Console.WriteLine($"AccessKey = {_endpointInfo.AccessKey}");
            Console.WriteLine();

            var validUntilUtc = DateTime.UtcNow + _endpointInfo.Ttl;

            var authorization = SharedAccessSignatureTokenProviderEx.GetSharedAccessSignature(
                _endpointInfo.KeyName,
                _endpointInfo.AccessKey,
                _endpointInfo.Resource,
                _endpointInfo.Ttl);

            var result = new AnalyticsEndpointInfoResponseModel()
            {
                HttpVerb = "POST",
                Url = _endpointInfo.Resource,
                ContentType = "application/json",
                Authorization = authorization,
                ValidUntilUtc = validUntilUtc
            };

            return Ok(result);
        }
    }
}

