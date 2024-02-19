using CopilotTest1.Core.Infrastructure;
using CopilotTest1.Core.Locations;

namespace CopilotTest1.Core.Domain.Locations
{
    public class LocationClosingAddedEvent : DomainEvent
    {
        public LocationClosing? Closing { get; set; }
    }
}