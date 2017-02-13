// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Common
{
    public static class StringExtensions
    {
        public static string EnsureEndsWith(this string value, string ending)
        {
            return value.EndsWith(ending) ? value : value + ending;
        }
    }
}
