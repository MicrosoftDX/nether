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
    public class ReturnValidationFailureOnInvalidModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!context.ModelState.IsValid)
            {
                var errorDetails = context.ModelState.SelectMany(keyValuePair =>
                    keyValuePair.Value.Errors.Select(error => new ErrorDetail(keyValuePair.Key, error.ErrorMessage))
                )
                .ToArray();

                context.Result = new BadRequestObjectResult(new ErrorModel
                {
                    Code = ErrorCode.ValidationFailed,
                    Message = DefaultErrorMessages.ValidationFailed,
                    Details = errorDetails
                });
            }
        }
    }
}
