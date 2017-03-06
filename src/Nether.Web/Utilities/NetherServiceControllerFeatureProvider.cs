// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Nether.Web.Utilities
{
    /// <summary>
    /// Filters out controllers with the NetherServiceAttribute if the service they represent isn't enabled in NetherServiceSwitchSettings
    /// </summary>
    public class NetherServiceControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly NetherServiceSwitchSettings _serviceSwitchSettings;

        public NetherServiceControllerFeatureProvider(NetherServiceSwitchSettings netherServiceSwitchSettings)
        {
            _serviceSwitchSettings = netherServiceSwitchSettings;
        }
        protected override bool IsController(TypeInfo typeInfo)
        {
            if (!base.IsController(typeInfo))
            {
                // Standard logic doesn't consider it a controller
                return false;
            }

            var netherServiceAttributeData = typeInfo.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(NetherServiceAttribute));

            if (netherServiceAttributeData == null)
            {
                // No NetherServiceAttribute specified, so not restricted by Nether Service
                return true;
            }

            // Get the serviceName and test if that service is enabled
            string serviceName = (string)netherServiceAttributeData.ConstructorArguments[0].Value;
            return _serviceSwitchSettings.IsServiceEnabled(serviceName);
        }
    }
}
