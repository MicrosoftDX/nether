// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Utilities
{
    public static class ControllerExtensions
    {
        public static IActionResult ValidationFailed(this ControllerBase controller, ErrorDetail errorDetail)
        {
            return controller.ValidationFailed(message: null, errorDetail: errorDetail);
        }
        public static IActionResult ValidationFailed(this ControllerBase controller, string message, ErrorDetail errorDetail)
        {
            return controller.ValidationFailed(message, new[] { errorDetail });
        }
        public static IActionResult ValidationFailed(this ControllerBase controller, params ErrorDetail[] errorDetails)
        {
            return controller.ValidationFailed(message: null, errorDetails: errorDetails);
        }
        public static IActionResult ValidationFailed(this ControllerBase controller, string message, params ErrorDetail[] errorDetails)
        {
            return controller.BadRequest(new ErrorModel
            {
                Code = ErrorCode.ValidationFailed,
                Message = message ?? DefaultErrorMessages.ValidationFailed,
                Details = errorDetails
            });
        }
    }
}
