// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.PlayerManagement.Models.PlayerManagementIntegration
{
    public class GamertagsLookupRequestModel
    {
        [Required]
        public string[] UserIds { get; set; }
    }
}
