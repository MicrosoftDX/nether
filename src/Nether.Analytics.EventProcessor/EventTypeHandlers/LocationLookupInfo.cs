namespace Nether.Analytics.EventProcessor.EventTypeHandlers
{
    public class LocationLookupInfo
    {
        public string Country { get; }
        public string District { get; }
        public string City { get; }

        public LocationLookupInfo(string country, string district, string city)
        {
            Country = country;
            District = district;
            City = city;
        }
    }


}
