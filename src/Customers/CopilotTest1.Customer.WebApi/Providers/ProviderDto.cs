using CopilotTest1.Shared.Domain.Infrastructure;

namespace CopilotTest1.People.WebApi.Providers
{
    public class ProviderDto
    {
        public ProviderProfileDto Profile { get; set; } = new ProviderProfileDto();

        public List<DayOfOperation> Hours { get; set; } = new List<DayOfOperation>();

        public List<Guid> Services { get; set; } = new List<Guid>();

        public bool IsActive { get; set; } = true;
    }
}
