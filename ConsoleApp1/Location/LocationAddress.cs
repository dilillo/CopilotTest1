using ConsoleApp1.Infrastructure;

namespace ConsoleApp1.Location
{
    public class LocationAddress
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public GeoCoordinate Coordinate { get; set; }
    }
}
