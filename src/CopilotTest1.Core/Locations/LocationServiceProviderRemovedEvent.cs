using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Locations
{
    public class LocationServiceProviderRemovedEvent : DomainEvent
    {
        public LocationServiceProvider? ServiceProvider { get; set; }
    }
}