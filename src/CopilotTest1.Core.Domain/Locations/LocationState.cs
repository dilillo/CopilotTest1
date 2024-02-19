using CopilotTest1.Core.Infrastructure;
using CopilotTest1.Core.Locations;

namespace CopilotTest1.Core.Domain.Locations
{
    [GenerateSerializer]
    public class LocationState
    {
        [Id(0)]
        public Guid BusinessId { get; set; }

        [Id(1)]
        public LocationProfile Profile { get; set; } = new LocationProfile();

        [Id(2)]
        public List<DayOfOperation> Hours { get; set; } = new List<DayOfOperation>();

        [Id(3)]
        public List<LocationService> Services { get; set; } = new List<LocationService>();

        [Id(4)]
        public List<LocationServiceProvider> ServiceProviders { get; set; } = new List<LocationServiceProvider>();

        [Id(5)]
        public bool IsPermanentlyClosed { get; set; } = false;

        public LocationState Apply(LocationOpenedEvent @event)
        {
            BusinessId = @event.BusinessId;
            Profile = @event.Profile;
            Hours = @event.Hours;

            return this;
        }

        public LocationState Apply(LocationProfileModifiedEvent @event)
        {
            Profile = @event.Profile;
            return this;
        }

        public LocationState Apply(LocationHoursModifiedEvent @event)
        {
            Hours = @event.Hours;
            return this;
        }

        public LocationState Apply(LocationServiceAddedEvent @event)
        {
            if (@event.Service == null)
                throw new ArgumentNullException(nameof(@event.Service));

            Services.Add(@event.Service);

            return this;
        }

        public LocationState Apply(LocationServiceModifiedEvent @event)
        {
            if (@event.Service == null)
                throw new ArgumentNullException(nameof(@event.Service));

            Services = Services.Where(x => x.Id != @event.Service.Id).ToList();

            Services.Add(@event.Service);

            return this;
        }

        public LocationState Apply(LocationServiceRemovedEvent @event)
        {
            Services = Services.Where(x => x.Id != @event.LocationServiceId).ToList();

            return this;
        }

        public LocationState Apply(LocationServiceProviderAddedEvent @event)
        {
            if (@event.ServiceProvider == null)
                throw new ArgumentNullException(nameof(@event.ServiceProvider));

            ServiceProviders.Add(@event.ServiceProvider);

            return this;
        }

        public LocationState Apply(LocationServiceProviderRemovedEvent @event)
        {
            if (@event.ServiceProvider == null)
                throw new ArgumentNullException(nameof(@event.ServiceProvider));

            ServiceProviders = ServiceProviders.Where(x => !(x.LocationServiceId == @event.ServiceProvider.LocationServiceId && x.ProviderId == @event.ServiceProvider.ProviderId)).ToList();

            return this;
        }

        public LocationState Apply(LocationPermanentlyClosedEvent @event)
        {
            IsPermanentlyClosed = true;

            return this;
        }
    }
}