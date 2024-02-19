using CopilotTest1.Core.Infrastructure;
using CopilotTest1.Core.Locations;

namespace CopilotTest1.Core.Domain.Locations
{
    public class LocationServiceModifiedEvent : DomainEvent
    {
        public LocationService? Service { get; set; }
    }
}