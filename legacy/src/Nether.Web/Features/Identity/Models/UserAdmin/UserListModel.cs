// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Nether.Web.Features.Identity.Models.UserAdmin
{
    public class UserListModel
    {
        public IEnumerable<UserSummaryModel> Users { get; set; }
    }
}
