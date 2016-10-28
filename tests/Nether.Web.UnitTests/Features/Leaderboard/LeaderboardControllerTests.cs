// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nether.Data.Leaderboard;
using Nether.Web.Features.Leaderboard;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Web.UnitTests.Features.Leaderboard
{
    public class LeaderboardControllerTests
    {
        // TODO - need to fix these to work with authentication!

        //[Fact(DisplayName = "WhenPostedScoreIsNegativeThenReturnHTTP400")]
        //public async Task WhenPostedScoreIsNegative_ThenTheApiReturns400Response()
        //{
        //    // Arrange
        //    var leaderboardStore = new Mock<ILeaderboardStore>();
        //    var controller = new LeaderboardController(leaderboardStore.Object, null);

        //    // Act
        //    var result = await controller.Post(new LeaderboardPostRequestModel
        //    {
        //        Gamertag = "anonymous",
        //        Score = -1
        //    });

        //    // Assert
        //    var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        //    Assert.Equal(400, statusCodeResult.StatusCode);
        //}

        //[Fact]
        //public async Task WhenPostedScoreIsNegative_ThenTheApiDoesNotSaveScore()
        //{
        //    // Arrange
        //    var leaderboardStore = new Mock<ILeaderboardStore>();
        //    var controller = new LeaderboardController(leaderboardStore.Object, null);

        //    // Act
        //    var result = await controller.Post(new LeaderboardPostRequestModel
        //    {
        //        Gamertag = "anonymous",
        //        Score = -1
        //    });

        //    // Assert
        //    leaderboardStore.Verify(o=>o.SaveScoreAsync(It.IsAny<GameScore>()), Times.Never);
        //}
    }
}

