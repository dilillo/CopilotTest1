namespace CopilotTest1.Core.Locations
{
    [GenerateSerializer]
    public class LocationServiceProvider
    {
        [Id(0)]
        public Guid LocationServiceId { get; set; }

        [Id(1)]
        public Guid ProviderId { get; set; }
    }
}
