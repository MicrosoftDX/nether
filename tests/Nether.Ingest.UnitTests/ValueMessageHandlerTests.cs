// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Xunit;

namespace Nether.Ingest.UnitTests
{
    public class ValueMessageHandlerTests
    {
        [Fact]
        public async void WhenGivenOnlyARandomIntGenerator_ShouldGenerateIntInDefaultProperty()
        {
            var handler = new ValueMessageHandler(new RandomIntValueGenerator());

            var msg = new Message();
            await handler.ProcessMessageAsync(msg, "pipeline", 0);

            Assert.Equal(1, msg.Properties.Count);
            Assert.True(msg.Properties.ContainsKey("rnd"), "Message should contain a property named 'rnd'");

            var s = msg.Properties["rnd"];
            var i = int.Parse(s);

            Assert.True(i >= 0);
        }

        [Fact]
        public async void WhenGivenOnlyAGuidGenerator_ShouldGenerateGuidInDefaultProperty()
        {
            var handler = new ValueMessageHandler(new GuidValueGenerator());

            var msg = new Message();
            await handler.ProcessMessageAsync(msg, "pipeline", 1);

            Assert.Equal(1, msg.Properties.Count);
            Assert.True(msg.Properties.ContainsKey("guid"), "Message should contain a property named 'guid'");

            var s = msg.Properties["guid"];
            var g = Guid.Parse(s);

            Assert.IsType(typeof(Guid), g);
        }
    }
}
