using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEventsGenerator
{
    public class Location
    {
        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
    
        public Location(string city, string latitude, string longitude, string country)
        {
            City = city;
            Latitude = latitude;
            Longitude = longitude;
            Country = country;
        }
    }
}
