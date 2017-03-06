// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Utilities
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NetherServiceAttribute : // ******************* Currently hides the api.... need to disable it! ***********
        Attribute,
        IFilterFactory
    {
        public string Name { get; private set; }

        public NetherServiceAttribute(string name)
        {
            Name = name;
        }


        bool IFilterFactory.IsReusable => false;

        IFilterMetadata IFilterFactory.CreateInstance(IServiceProvider services)
        {
            // TODO resolve FeatureConfiguration class (needs creating, configuring and registering!)
            var serviceSwitchSettings = services.GetRequiredService<NetherServiceSwitchSettings>();
            return new InternalNetherServiceAttribute(serviceSwitchSettings, Name);
        }

        private class InternalNetherServiceAttribute : Attribute, IApiDescriptionVisibilityProvider, IFilterMetadata
        {
            public string _name;
            private NetherServiceSwitchSettings _serviceSwitchSettings;

            public InternalNetherServiceAttribute(NetherServiceSwitchSettings serviceSwitchSettings, string name)
            {
                _serviceSwitchSettings = serviceSwitchSettings;
                _name = name;
            }


            bool IApiDescriptionVisibilityProvider.IgnoreApi
            {
                get { return !_serviceSwitchSettings.IsServiceEnabled(_name); }
            }
        }
    }
}
