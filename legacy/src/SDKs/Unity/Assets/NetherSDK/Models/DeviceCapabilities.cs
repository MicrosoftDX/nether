// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetherSDK.Models
{
    [Serializable]
    public class DeviceCapabilities
    {
        public string cpu;
        public string ram;
        public string resolution;
        public string deviceName;
    }
}
