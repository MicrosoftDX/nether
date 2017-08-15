// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity.Models.UserAdmin
{
    public class UserSummaryModel
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        [JsonProperty(PropertyName = "_link")]
        public string _Link { get; set; }
    }
}
