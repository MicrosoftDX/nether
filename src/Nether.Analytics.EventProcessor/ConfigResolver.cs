// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Analytics.EventProcessor
{
    public static class ConfigResolver
    {
        public static string Resolve(string name)
        {
            var configVar = Environment.GetEnvironmentVariable(name);
            return configVar ?? ConfigurationManager.AppSettings[name].ToString();
        }
    }
}