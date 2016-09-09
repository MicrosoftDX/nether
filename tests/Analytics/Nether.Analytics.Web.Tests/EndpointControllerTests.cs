using Microsoft.AspNetCore.Mvc;
using Nether.Analytics.Web.Controllers;
using System.Threading.Tasks;
using Xunit;

namespace Nether.Analytics.Web.Tests
{
    public class EndpointControllerTests
    {
        [Fact]
        public async Task ShouldBeAbleToRetrieveEndpointInformation()
        {
            // Arrange
            var controller = new EndpointController();

            // Act
            var result = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            

        }
    }
}
