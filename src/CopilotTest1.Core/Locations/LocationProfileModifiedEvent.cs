using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Locations
{
    public class LocationProfileModifiedEvent : DomainEvent
    {
        public LocationProfile? Profile { get; set; }
    }
}