using Microsoft.AspNetCore.Mvc;
using Moq;
using Nether.Leaderboard.Data;
using Nether.Leaderboard.Web.Controllers;
using Nether.Leaderboard.Web.Models;
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
            var result = await controller.Post(new LeaderboardPostRequestModel
            {
                Gamertag = "anonymous",
                Score = -1
            }); 

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
            var result = await controller.Post(new LeaderboardPostRequestModel
            {
                Gamertag = "anonymous",
                Score = -1
            });

            // Assert
            leaderboardStore.Verify(o=>o.SaveScoreAsync(It.IsAny<GameScore>()), Times.Never);
        }
    }
}
