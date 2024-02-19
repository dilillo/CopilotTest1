using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Providers
{
    [GenerateSerializer]
    public class ProviderLocationDayOfOperation : DayOfOperation
    {
        [Id(0)]
        public Guid LocationId { get; set; }
    }
}
