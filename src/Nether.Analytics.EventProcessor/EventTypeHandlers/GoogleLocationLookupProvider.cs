using System;
using System.Threading.Tasks;

namespace Nether.Analytics.EventProcessor.EventTypeHandlers
{
    public class GoogleLocationLookupProvider : ILocationLookupProvider
    {
        Task<LocationLookupInfo> ILocationLookupProvider.Lookup(double lat, double lon)
        {
            throw new NotImplementedException();
        }
    }


}
