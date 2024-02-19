using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Providers
{
    public class ProviderHoursModifiedEvent : DomainEvent
    {
        public List<ProviderLocationDayOfOperation> Hours { get; set; } = new List<ProviderLocationDayOfOperation>();
    }
}