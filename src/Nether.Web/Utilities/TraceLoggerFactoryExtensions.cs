using Nether.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging
{
    public static class TraceLoggerFactoryExtensions
    {
        public static ILoggerFactory AddTrace(this ILoggerFactory factory)
        {
            return AddTrace(factory, LogLevel.Information);
        }

        public static ILoggerFactory AddTrace(this ILoggerFactory factory, Func<string, LogLevel, bool> filter)
        {
            factory.AddProvider(new TraceLoggerProvider(filter));
            return factory;
        }

        public static ILoggerFactory AddTrace(this ILoggerFactory factory, LogLevel minLevel)
        {
            return AddTrace(
               factory,
               (_, logLevel) => logLevel >= minLevel);
        }
    }
}
