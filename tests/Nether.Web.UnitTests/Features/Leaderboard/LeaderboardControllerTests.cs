// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using Nether.Data.Leaderboard;
using Nether.Integration.Analytics;
using Nether.Web.Features.Leaderboard;
using System;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using IdentityModel;

namespace Nether.Web.UnitTests.Features.Leaderboard
{
    public class LeaderboardControllerTests
    {
        [Fact(DisplayName = "WhenPostedScoreIsNegativeThenReturnHTTP400")]
        public async Task WhenPostedScoreIsNegative_ThenTheApiReturns400Response()
        {
            // Arrange
            var leaderboardStore = new Mock<ILeaderboardStore>();
            var controller = CreateController<LeaderboardController>(
                services =>
                {
                    services.Setup(s => s.GetService(typeof(ILeaderboardStore))).Returns(leaderboardStore.Object);
                    services.Setup(s => s.GetService(typeof(IAnalyticsIntegrationClient))).Returns(Mock.Of<IAnalyticsIntegrationClient>());
                });

            // Act
            var result = await controller.Post(new LeaderboardPostRequestModel
            {
                Score = -1
            });

            // Assert
            var statusCodeResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task WhenPostedScoreIsNegative_ThenTheApiDoesNotSaveScore()
        {
            // Arrange
            var leaderboardStore = new Mock<ILeaderboardStore>();
            var controller = CreateController<LeaderboardController>(
                services =>
                {
                    services.Setup(s => s.GetService(typeof(ILeaderboardStore))).Returns(leaderboardStore.Object);
                    services.Setup(s => s.GetService(typeof(IAnalyticsIntegrationClient))).Returns(Mock.Of<IAnalyticsIntegrationClient>());
                }
            );


            // Act
            var result = await controller.Post(new LeaderboardPostRequestModel
            {
                //Gamertag = "anonymous",
                Score = -1
            });

            // Assert
            leaderboardStore.Verify(o => o.SaveScoreAsync(It.IsAny<GameScore>()), Times.Never);
        }


        [Fact]
        public async Task WhenPostedScoreIsValid_ThenTheApiDoesSavesScore()
        {
            // Arrange
            var leaderboardStore = new Mock<ILeaderboardStore>();
            var controller = CreateController<LeaderboardController>(
                services =>
                {
                    services.Setup(s => s.GetService(typeof(ILeaderboardStore))).Returns(leaderboardStore.Object);
                    services.Setup(s => s.GetService(typeof(IAnalyticsIntegrationClient))).Returns(Mock.Of<IAnalyticsIntegrationClient>());
                },
                user: new ClaimsPrincipal(
                        new ClaimsIdentity(new[]
                            {
                                new Claim(ClaimTypes.Name, "a-user"),
                                new Claim(JwtClaimTypes.NickName, "a-user")
                            })
                ));


            // Act
            var result = await controller.Post(new LeaderboardPostRequestModel
            {
                //Gamertag = "anonymous",
                Score = 10
            });

            // Assert
            var statusCodeResult = Assert.IsType<OkResult>(result);
            leaderboardStore.Verify(o => o.SaveScoreAsync(It.IsAny<GameScore>()), Times.Once);
        }


        // Helpers based on https://github.com/aspnet/Mvc/blob/0eea3c2651ac3999d790b91d3fb43d840944f4ec/test/Microsoft.AspNetCore.Mvc.Core.Test/Controllers/DefaultControllerFactoryTest.cs
        public TController CreateController<TController>(Action<Mock<IServiceProvider>> serviceRegistration, ClaimsPrincipal user = null)
        {
            var actionDescriptor = new ControllerActionDescriptor
            {
                ControllerTypeInfo = typeof(TController).GetTypeInfo()
            };

            var context = new ControllerContext()
            {
                ActionDescriptor = actionDescriptor,
                HttpContext = new DefaultHttpContext()
                {
                    RequestServices = GetServices(serviceRegistration),
                    User = user
                },
            };
            var factory = CreateControllerFactory(new DefaultControllerActivator(new TypeActivatorCache()));

            var result = (TController)factory.CreateController(context);
            return result;
        }
        private static DefaultControllerFactory CreateControllerFactory(IControllerActivator controllerActivator = null)
        {
            var activatorMock = new Mock<IControllerActivator>();

            controllerActivator = controllerActivator ?? activatorMock.Object;
            var propertyActivators = new IControllerPropertyActivator[]
            {
                new DefaultControllerPropertyActivator(),
            };

            return new DefaultControllerFactory(controllerActivator, propertyActivators);
        }
        private IServiceProvider GetServices(Action<Mock<IServiceProvider>> serviceRegistration)
        {
            var services = new Mock<IServiceProvider>();
            serviceRegistration(services);
            return services.Object;
        }
    }
}

