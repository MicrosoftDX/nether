using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.IntegrationTests
{
    public static class StringExtensions
    {
        public static string EnsureEndsWith(this string value, string ending)
        {
            return value.EndsWith(ending) ? value : value + ending;
        }
    }
}
