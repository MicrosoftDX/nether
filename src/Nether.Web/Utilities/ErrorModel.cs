// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Utilities
{
    // minimal error model inpsired by https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#7102-error-condition-responses

    public class ErrorModel
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorCode Code { get; set; }

        public string Message { get; set; }
    }

    public enum ErrorCode
    {
        UnhandledException
    }
}
