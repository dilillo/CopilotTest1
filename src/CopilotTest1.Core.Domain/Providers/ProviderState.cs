using CopilotTest1.Core.Providers;

namespace CopilotTest1.Core.Domain.Providers
{
    [GenerateSerializer]
    public class ProviderState
    {
        [Id(1)]
        public List<ProviderLocationDayOfOperation> Hours { get; set; } = new List<ProviderLocationDayOfOperation>();

        public ProviderState Apply(ProviderHoursModifiedEvent providerHoursModifiedEvent)
        {
            Hours = providerHoursModifiedEvent.Hours;

            return this;
        }
    }
}
