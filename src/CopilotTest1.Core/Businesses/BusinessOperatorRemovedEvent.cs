using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Businesses
{
    public class BusinessOperatorRemovedEvent : DomainEvent
    {
        public Guid OperatorId { get; set; }
    }
}