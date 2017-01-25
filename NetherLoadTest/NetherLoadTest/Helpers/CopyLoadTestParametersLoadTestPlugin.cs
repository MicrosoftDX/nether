// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.LoadTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetherLoadTest.Helpers
{
    [DisplayName("Copy load test parameters")]
    [Description("Copies the load test context parameters to the test context for individual tests")]
    public class CopyLoadTestParametersLoadTestPlugin : ILoadTestPlugin
    {
        private LoadTest _loadTest;

        public void Initialize(LoadTest loadTest)
        {
            _loadTest = loadTest;

            loadTest.TestStarting += LoadTest_TestStarting;
        }

        private void LoadTest_TestStarting(object sender, TestStartingEventArgs e)
        {
            foreach (var property in _loadTest.Context)
            {
                e.TestContextProperties.Add(property);
            }
        }
    }
}
