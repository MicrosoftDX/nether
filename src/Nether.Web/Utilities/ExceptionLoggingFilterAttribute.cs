// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nether.Web.Utilities
{
    public class ExceptionLoggingFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ExceptionLoggingFilterAttribute(
            ILogger<ExceptionLoggingFilterAttribute> logger,
            IHostingEnvironment hostingEnvironment
            )
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            string message;
            if (_hostingEnvironment.EnvironmentName == "Development")
            {
                // For development, write full exception message to client
                _logger.LogError("Unhandled exception: {0}", context.Exception);
                message = context.Exception.ToString();
            }
            else
            {
                // For non-development, write full exception message to log with identifier
                // and write identifier to client
                var id = Guid.NewGuid().ToString();
                _logger.LogError("Unhandled exception (error-id = {0}): {1}", id, context.Exception);
                message = $"Unhandled exception. Check logs for error id '{id}'";
            }

            var error = new ErrorModel
            {
                Code = ErrorCode.UnhandledException,
                Message = message
            };
            context.Result = new ObjectResult(error) { StatusCode = (int)HttpStatusCode.InternalServerError };
        }
    }
}
