namespace CopilotTest1.Core.Locations
{
    [GenerateSerializer]
    public class LocationService
    {
        [Id(0)]
        public  Guid Id { get; set; }

        [Id(1)]
        public string Name { get; set; } = string.Empty;

        [Id(2)]
        public string Description { get; set; } = string.Empty;

        [Id(3)]
        public int DurationInMinutes { get; set; }
    }
}
