// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.DataLake;
using System;
using System.Collections.Generic;
using Xunit;

namespace Nether.Ingest.DataLake.UnitTests
{
    public class UsqlHelperTests
    {
        // Due to the nature of U-SQL Scripts the input and expected output of these Unit Tests has been
        // placed in textfiles that are accessed using Resource Manager. Please look at the Scripts folder
        // to see actual scripts.

        [Fact]
        public void WhenParameterDoesNotExist_LeaveScriptIntact()
        {
            var script = "DECLARE @YEAR int = 1974; DECLARE @MONTH int = 4;" + Environment.NewLine +
                         "DECLARE @TEST string = \"Hello World!\";" + Environment.NewLine +
                         "... more of the script goes here ...";

            var expectedScript = script;

            var result = UsqlHelper.ReplaceVariableValueInScript(script, "@XYZ", 4711, out var variableFound);

            Assert.False(variableFound);
            Assert.Equal(expectedScript, result);
        }

        [Fact]
        public void WhenOneNumericParameterExists_ReplaceItsValue()
        {
            var script = "DECLARE @YEAR int = 1974; DECLARE @MONTH int = 4;" + Environment.NewLine +
                         "DECLARE @TEST string = \"Hello World!\";" + Environment.NewLine +
                         "... more of the script goes here ...";

            var expectedScript = "DECLARE @YEAR int = 1974; DECLARE @MONTH int = 8;" + Environment.NewLine +
                         "DECLARE @TEST string = \"Hello World!\";" + Environment.NewLine +
                         "... more of the script goes here ...";

            var result = UsqlHelper.ReplaceVariableValueInScript(script, "@MONTH", 8, out var variableFound);

            Assert.True(variableFound);
            Assert.Equal(expectedScript, result);
        }

        [Fact]
        public void WhenOneStringParameterExists_ReplaceItsValue()
        {
            var script = "DECLARE @YEAR int = 1974; DECLARE @MONTH int = 4;" + Environment.NewLine +
                         "DECLARE @TEST string = \"Hello World!\";" + Environment.NewLine +
                         "... more of the script goes here ...";

            var expectedScript = "DECLARE @YEAR int = 1974; DECLARE @MONTH int = 4;" + Environment.NewLine +
                         "DECLARE @TEST string = \"Nether was here\";" + Environment.NewLine +
                         "... more of the script goes here ...";

            var result = UsqlHelper.ReplaceVariableValueInScript(script, "@TEST", "Nether was here", out var variableFound);

            Assert.True(variableFound);
            Assert.Equal(expectedScript, result);
        }

        [Fact]
        public void WhenManyParametersExist_ReplaceAllTheirValues()
        {
            var script = "DECLARE @YEAR int = 1974; DECLARE @MONTH int = 4;" + Environment.NewLine +
                         "DECLARE @TEST string = \"Hello World!\";" + Environment.NewLine +
                         "... more of the script goes here ...";

            var expectedScript = "DECLARE @YEAR int = 1974; DECLARE @MONTH int = 5;" + Environment.NewLine +
                         "DECLARE @TEST string = \"/nether/clustering\";" + Environment.NewLine +
                         "... more of the script goes here ...";

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
