// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using System;
using Nether.Data.PlayerManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Nether.Data.EntityFramework.PlayerManagement
{
    [DebuggerDisplay("PlayerExtendedEntity ('{UserId}')")]
    public class PlayerExtendedEntity
    {
        public string UserId { get; set; }
        public string State { get; set; }
    }
}
