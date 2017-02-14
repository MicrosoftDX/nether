using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Utilities
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Reads a string array from a configuration section
        /// If the value is an array then it returns the array
        /// If the value is a string then it parses it as comma-separated string and returns the array
        /// </summary>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IEnumerable<string> ParseStringArray(this IConfigurationSection configSection)
        {
            if (configSection.Value == null)
            {
                // when the config is specified using JSON it can come as child config elements
                // if specified in an arry
                return configSection.GetChildren()
                    .Select(v => v.Value);
            }
            else
            {
                // when specified via environment variables it comes in as a comma-delimited string
                return configSection.Value
                    .Split(',')
                    .Select(s => s.Trim());
            }
        }
        /// <summary>
        /// Reads a string array from a child configuration section
        /// If the value is an array then it returns the array
        /// If the value is a string then it parses it as comma-separated string and returns the array
        /// </summary>
        /// <param name="configSection"></param>
        /// <param name="key">The key for the child section</param>
        /// <returns></returns>
        public static IEnumerable<string> ParseStringArray(this IConfigurationSection configSection, string key)
        {
            return configSection
                .GetSection(key)
                .ParseStringArray();
        }
    }
}
