using System.Threading.Tasks;

namespace Nether.Analytics.EventProcessor.EventTypeHandlers
{
    public class NullLocationLookupProvider : ILocationLookupProvider
    {
        public Task<LocationLookupInfo> Lookup(double lat, double lon)
        {
            return Task.FromResult(new LocationLookupInfo("NOT_SETUP_COUNTRY", "NOT_SETUP_DISTRICT", "NOT_SETUP_CITY"));
        }
    }


}
