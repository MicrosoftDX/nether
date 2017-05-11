// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Analytics.DataLake.UnitTests.Properties;
using System;
using System.Collections.Generic;
using Xunit;

namespace Nether.Analytics.DataLake.UnitTests
{
    public class UsqlHelperTests
    {
        // Due to the nature of U-SQL Scripts the input and expected output of these Unit Tests has been
        // placed in textfiles that are accessed using Resource Manager. Please look at the Scripts folder
        // to see actual scripts.

        [Fact]
        public void WhenParameterDoesNotExist_LeaveScriptIntact()
        {
            var script = Resources.Script001;
            var expectedScript = script;

            var result = UsqlHelper.ReplaceVariableValueInScript(script, "@XYZ", 4711, out var variableFound);

            Assert.False(variableFound);
            Assert.Equal(expectedScript, result);
        }

        [Fact]
        public void WhenOneNumericParameterExists_ReplaceItsValue()
        {
            var script = Resources.Script001;
            var expectedScript = Resources.Script001a;

            var result = UsqlHelper.ReplaceVariableValueInScript(script, "@MONTH", 8, out var variableFound);

            Assert.True(variableFound);
            Assert.Equal(expectedScript, result);
        }

        [Fact]
        public void WhenOneStringParameterExists_ReplaceItsValue()
        {
            var script = Resources.Script001;
            var expectedScript = Resources.Script001b;

            var result = UsqlHelper.ReplaceVariableValueInScript(script, "@TEST", "Nether was here!", out var variableFound);

            Assert.True(variableFound);
            Assert.Equal(expectedScript, result);
        }

        [Fact]
        public void WhenManyParametersExist_ReplaceAllTheirValues()
        {
            var script = Resources.Script001;
            var expectedScript = Resources.Script001c;

            var result = UsqlHelper.ReplaceVariableValuesInScript(script, new Dictionary<string, object>
            {
                {"@MONTH", 5 },
                {"@TEST", "/nether/clustering" }
            }, out var variablesFound);

            Assert.Equal("@MONTH", variablesFound[0]);
            Assert.Equal("@TEST", variablesFound[1]);
            Assert.Equal(2, variablesFound.Length);
            Assert.Equal(expectedScript, result);
        }
    }
}
