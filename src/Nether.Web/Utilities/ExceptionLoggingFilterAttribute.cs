// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Utilities
{
    public class ExceptionLoggingFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionLoggingFilterAttribute> _logger;

        public ExceptionLoggingFilterAttribute(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExceptionLoggingFilterAttribute>();
        }
        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception.ToString());
        }
    }
}
