using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Locations
{
    public class LocationHoursModifiedEvent : DomainEvent
    {
        public List<DayOfOperation> Hours { get; set; } = new List<DayOfOperation>();
    }
}