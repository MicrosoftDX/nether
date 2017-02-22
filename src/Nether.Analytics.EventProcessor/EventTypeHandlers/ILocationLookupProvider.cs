using System.Threading.Tasks;

namespace Nether.Analytics.EventProcessor.EventTypeHandlers
{
    public interface ILocationLookupProvider
    {
        Task<LocationLookupInfo> Lookup(double lat, double lon);
    }


}
