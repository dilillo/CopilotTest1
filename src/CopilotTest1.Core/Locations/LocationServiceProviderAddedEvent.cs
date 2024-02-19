using CopilotTest1.Core.Infrastructure;
using CopilotTest1.Core.Locations;

namespace CopilotTest1.Core.Domain.Locations
{
    public class LocationServiceProviderAddedEvent : DomainEvent
    {
        public LocationServiceProvider? ServiceProvider { get;  set; }
    }
}