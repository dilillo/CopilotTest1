using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Businesses
{
    public class BusinessOpenedEvent : DomainEvent
    {
        public BusinessProfile Profile { get; set; } = new BusinessProfile();

        public Guid OperatorId { get; set; }
    }
}