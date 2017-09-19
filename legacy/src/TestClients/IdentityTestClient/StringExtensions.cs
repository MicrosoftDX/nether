// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityTestClient
{
    public static class StringExtensions
    {
        public static string EnsureTrailingSlash(this string value)
        {
            if (value == null)
                return null;
            if (value.EndsWith("/"))
                return value;
            return value + "/";
        }
    }
}
