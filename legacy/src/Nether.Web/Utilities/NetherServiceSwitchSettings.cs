// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Utilities
{
    public class NetherServiceSwitchSettings
    {
        private readonly Dictionary<string, bool> _serviceSwitches = new Dictionary<string, bool>();
        public void AddServiceSwitch(string serviceName, bool enabled)
        {
            _serviceSwitches.Add(serviceName, enabled);
        }
        public bool IsServiceEnabled(string serviceName)
        {
            if (_serviceSwitches.TryGetValue(serviceName, out bool enabled))
            {
                return enabled;
            }
            return false; // assume not enabled if not registered
        }
    }
}
