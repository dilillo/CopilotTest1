using CopilotTest1.Shared.Domain.Infrastructure;
using CopilotTest1.Shared.Domain.Providers;

namespace CopilotTest1.People.Domain.Providers
{
    public class ProviderState
    {
        public ProviderProfile Profile { get; set; } = new ProviderProfile();

        public List<DayOfOperation> Hours { get; set; } = new List<DayOfOperation>();

        public List<Guid> Services { get; set; } = new List<Guid>();

        public bool IsActive { get; set; } = true;

        public ProviderState Apply(ProviderRegisteredEvent providerRegisteredEvent)
        {
            Profile = providerRegisteredEvent.Profile;

            return this;
        }   

        public ProviderState Apply(ProviderProfileModifiedEvent providerProfileModifiedEvent)
        {
            Profile = providerProfileModifiedEvent.Profile;

            return this;
        }

        public ProviderState Apply(ProviderDeactivatedEvent providerDeactivatedEvent)
        {
            IsActive = false;

            return this;
        }

        public ProviderState Apply(ProviderServiceAddedEvent providerServiceAddedEvent)
        {
            Services.Add(providerServiceAddedEvent.LocationServiceId);

            return this;
        }   

        public ProviderState Apply(ProviderServiceRemovedEvent providerServiceRemovedEvent)
        {
            Services.Remove(providerServiceRemovedEvent.LocationServiceId);

            return this;
        }

        public ProviderState Apply(ProviderHoursModifiedEvent providerHoursModifiedEvent)
        {
            Hours = providerHoursModifiedEvent.Hours;

            return this;
        }
    }
}
