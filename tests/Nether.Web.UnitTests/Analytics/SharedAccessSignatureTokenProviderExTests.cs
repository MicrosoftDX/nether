using System;
using Nether.Analytics.Web.Utilities;
using Xunit;

namespace Nether.Analytics.Web.Tests
{
    public class SharedAccessSignatureTokenProviderExTests
    {
        [Fact]
        public void ShouldBeAbleToGetSharedAccessSignature()
        {
            // Arrange
            var servicebusNamespace = "myservicebusnamespace";
            var eventhubName = "eventhubname";
            var keyName = "nether";
            var sharedAccessKey = "w8UXwPyDp6a0oxbeUyFoy6HEUOzJ0cYnVjt7muyzps4=";
            var resource = $"https://{servicebusNamespace}.servicebus.windows.net/{eventhubName}/messages";
            var tokenTimeToLive = TimeSpan.FromDays(365);

            // Act
            var sasToken = SharedAccessSignatureTokenProviderEx.GetSharedAccessSignature(keyName, sharedAccessKey, resource, tokenTimeToLive);

            // Assert
            
            // Signature will come out differently every time, hence the need to use RegEx to validate the token and not a fixed string
            Assert.Matches($"SharedAccessSignature sr=https%3A%2F%2F{servicebusNamespace}.servicebus.windows.net%2F{eventhubName}%2Fmessages&sig=.*&se=.*&skn={keyName}",
                sasToken);

        }
    }
}