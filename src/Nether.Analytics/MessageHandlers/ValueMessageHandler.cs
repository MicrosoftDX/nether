// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using System.Collections.Generic;

namespace Nether.Analytics
{
    /// <summary>
    /// Generic MessageHandler for quick none asynchronous generation of property values
    /// </summary>
    public class ValueMessageHandler : IMessageHandler
    {
        private IValueGenerator _generator;
        private string[] _properties;

        /// <summary>
        /// Constructs a new ValueGeneratorMessageHandler
        /// </summary>
        /// <param name="generator">The IPropertyValueGenerator to use for generation of value(s)</param>
        /// <param name="properties">A list of property names that should receive generated values. If omitted, the specified generators default value will be used.</param>
        public ValueMessageHandler(IValueGenerator generator, params string[] properties)
        {
            _generator = generator;
            _properties = properties;
        }

        public Task<MessageHandlerResults> ProcessMessageAsync(Message msg, string pipelineName, int index)
        {
            if (_properties.Length == 0)
            {
                msg.Properties[_generator.DefaultProperty] = _generator.GeneratePropertyValue();
            }
            else
            {
                foreach (var property in _properties)
                {
                    msg.Properties[property] = _generator.GeneratePropertyValue();
                }
            }

            return Task.FromResult(MessageHandlerResults.Success);
        }
    }

    public interface IValueGenerator
    {
        string DefaultProperty { get; }
        string GeneratePropertyValue();
    }
}
