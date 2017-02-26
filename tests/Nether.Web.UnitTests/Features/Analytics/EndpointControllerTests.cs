// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Nether.Web.Features.Analytics;
using System;
using Xunit;
using Nether.Web.Features.Analytics.Models.Endpoint;

namespace Nether.Web.UnitTests.Features.Analytics
{
    public class EndpointControllerTests
    {
        [Fact]
        public void ShouldBeAbleToRetrieveEndpointInformation()
        {
            // Arrange
            var controller = new EndpointController(new EndpointInfo
            {
                KeyName = "qwerty",
                AccessKey = "qwerty",
                Resource = "qwerty",
                Ttl = TimeSpan.FromMinutes(10),
            });

            // Act
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = okResult.Value as AnalyticsEndpointInfoResponseModel;
            Assert.NotNull(model);

            //TODO: Add additional asserts as soon as we can handle configurations witin unit tests
            Assert.Equal("POST", model.HttpVerb);
            Assert.NotEmpty(model.Url);
            Assert.Equal("application/json", model.ContentType);
            Assert.NotEmpty(model.Authorization);
        }
    }
}

