using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Domain.Locations
{
    public class LocationCustomerAddedEvent : DomainEvent
    {
        public Guid CustomerId { get; set; }
    }
}