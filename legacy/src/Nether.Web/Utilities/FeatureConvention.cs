// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Nether.Web.Utilities
{
    public class FeatureConvention : IControllerModelConvention
    {
        // based on https://github.com/ardalis/OrganizingAspNetCore/blob/05245bbaffdc48e7cc21312f97faf84738804c64/src/WithFeatureFolders/FeatureConvention.cs (see also https://msdn.microsoft.com/en-us/magazine/mt763233.aspx)

        public void Apply(ControllerModel controller)
        {
            controller.Properties.Add("feature", GetFeatureName(controller.ControllerType));
        }
        private string GetFeatureName(TypeInfo controllerType)
        {
            string[] tokens = controllerType.FullName.Split('.');
            if (!tokens.Any(t => t == "Features"))
            {
                return "";
            }
            string featureName = tokens
              .SkipWhile(t => !t.Equals("features", StringComparison.CurrentCultureIgnoreCase))
              .Skip(1)
              .Take(1)
              .FirstOrDefault();
            return featureName;
        }
    }
}

