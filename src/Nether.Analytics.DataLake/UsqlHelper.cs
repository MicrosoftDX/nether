// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nether.Analytics.DataLake
{
    public class UsqlHelper
    {
        public static string ReplaceVariableValuesInScript(string script, params Tuple<string, object>[] variableValuePairs)
        {
            return ReplaceVariableValuesInScript(script, variableValuePairs.ToDictionary(t => t.Item1, t => t.Item2), out var variablesFound);
        }

        public static string ReplaceVariableValuesInScript(string script, Dictionary<string, object> variables, out string[] variablesFound)
        {
            var listOfVariablesFound = new List<string>();

            foreach (var variable in variables)
            {
                script = ReplaceVariableValueInScript(script, variable.Key, variable.Value, out var variableFound);
                if (variableFound)
                {
                    listOfVariablesFound.Add(variable.Key);
                }
            }

            // Set out parameter
            variablesFound = listOfVariablesFound.ToArray();
            return script;
        }

        public static string ReplaceVariableValueInScript(string script, string variable, object value, out bool variableFound)
        {
            //TODO: Find better regex that allows string variables in just one replace statement
            // Current implementation doesn't isolate content of quoted values, therefor the extra
            // check to see if value is string or not

            var pattern = @"(?<=DECLARE\s*" + variable + @"((\s*)|(\s.*))=\s*)([\S].*)(?=\s*;)";

            var matches = Regex.Matches(script, pattern, RegexOptions.Multiline);
            if (matches.Count == 0)
            {
                variableFound = false;
                return script;
            }
            else if (matches.Count == 1)
            {
                variableFound = true;

                if (value is string)
                {
                    return Regex.Replace(script, pattern, "\"" + value + "\"");
                }
                else
                {
                    return Regex.Replace(script, pattern, value.ToString());
                }
            }
            else
            {
                throw new Exception($"Variable {variable} was found in more than one place in the script");
            }
        }
    }
}
