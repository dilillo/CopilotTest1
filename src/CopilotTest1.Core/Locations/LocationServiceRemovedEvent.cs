using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Locations
{
    public class LocationServiceRemovedEvent : DomainEvent
    {
        public Guid LocationServiceId { get; set; }
    }
}