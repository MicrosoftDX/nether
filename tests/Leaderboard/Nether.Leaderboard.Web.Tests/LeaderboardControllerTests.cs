using Microsoft.AspNetCore.Mvc;
using Moq;
using Nether.Leaderboard.Data;
using Nether.Leaderboard.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Leaderboard.Web.Tests
{
    public class LeaderboardControllerTests
    {
        [Fact(DisplayName = "WhenPostedScoreIsNegativeThenReturnHTTP400")]
        public async Task WhenPostedScoreIsNegative_ThenTheApiReturns400Response()
        {
            // Arrange
            var leaderboardStore = new Mock<ILeaderboardStore>();
            var controller = new LeaderboardController(leaderboardStore.Object);

            // Act
            var result = await controller.Post(-1);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(400, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task WhenPostedScoreIsNegative_ThenTheApiDoesNotSaveScore()
        {
            // Arrange
            var leaderboardStore = new Mock<ILeaderboardStore>();
            var controller = new LeaderboardController(leaderboardStore.Object);

            // Act
            var result = await controller.Post(-1);

            // Assert
            leaderboardStore.Verify(o => o.SaveScoreAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }
    }
}
