using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Domain.Locations
{
    public class LocationCustomerRemovedEvent : DomainEvent
    {
        public Guid CustomerId { get;  set; }
    }
}