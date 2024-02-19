using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Locations
{
    public class LocationOpenedEvent : DomainEvent
    {
        public LocationProfile Profile { get; set; } = new LocationProfile();

        public Guid BusinessId { get; set; }

        public List<DayOfOperation> Hours { get; set; } = new List<DayOfOperation>();
    }
}