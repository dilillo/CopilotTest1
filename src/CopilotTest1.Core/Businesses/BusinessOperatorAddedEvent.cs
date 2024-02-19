using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Businesses
{
    public class BusinessOperatorAddedEvent : DomainEvent
    {
        public Guid OperatorId { get; set; }
    }
}