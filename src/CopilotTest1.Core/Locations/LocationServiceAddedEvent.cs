using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Locations
{
    public class LocationServiceAddedEvent : DomainEvent
    {
        public LocationService? Service { get; set; }
    }
}