using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Locations
{
    [GenerateSerializer]
    public class LocationAddress
    {
        [Id(0)]
        public string AddressLine1 { get; set; } = string.Empty;

        [Id(1)]
        public string AddressLine2 { get; set; } = string.Empty;

        [Id(2)]
        public string City { get; set; } = string.Empty;

        [Id(3)]
        public string PostalCode { get; set; } = string.Empty;

        [Id(4)]
        public GeoCoordinate Coordinate { get; set; }
    }
}
