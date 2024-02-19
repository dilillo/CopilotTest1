namespace CopilotTest1.Core.Locations
{
    [GenerateSerializer]
    public class LocationProfile
    {
        [Id(0)]
        public string Name { get; set; } = string.Empty;

        [Id(1)]
        public string Email { get; set; } = string.Empty;

        [Id(2)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Id(3)]
        public LocationAddress Address { get; set; } = new LocationAddress();
    }
}
