using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Domain.Providers
{
    public class ProviderTimeOffCancelledEvent : DomainEvent
    {
        public Guid ProviderTimeOffId { get; set; }
    }
}