using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Domain.Locations
{
    public class LocationClosingRemovedEvent : DomainEvent
    {
        public Guid LocationClosingId { get;  set; }
    }
}