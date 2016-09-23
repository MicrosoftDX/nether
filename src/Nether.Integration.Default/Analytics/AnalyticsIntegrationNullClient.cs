using System.Threading.Tasks;
using Nether.Integration.Analytics;

namespace Nether.Integration.Default.Analytics
{
    public class AnalyticsIntegrationNullClient : IAnalyticsIntegrationClient
    {
        public async Task SendGameEventAsync(GameEvent gameEvent)
        {
            await Task.CompletedTask;
        }
    }
}