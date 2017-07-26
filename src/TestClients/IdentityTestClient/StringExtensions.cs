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
