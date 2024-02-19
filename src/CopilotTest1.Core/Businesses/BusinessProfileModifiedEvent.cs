using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Businesses
{
    public class BusinessProfileModifiedEvent : DomainEvent
    {
        public BusinessProfile? Profile { get; set; }
    }
}