// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Common.ApplicationPerformanceMonitoring
{
    public interface IApplicationPerformanceMonitor
    {
        void LogEvent(string eventName, string message = null, IDictionary<string, string> properties = null);
        void LogError(Exception exception, string message = null, IDictionary<string, string> properties = null);
    }
}