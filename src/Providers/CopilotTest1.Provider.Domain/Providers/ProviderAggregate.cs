using CopilotTest1.People.Data;
using CopilotTest1.People.Data.Entities;
using CopilotTest1.Shared.Domain.Infrastructure;
using CopilotTest1.Shared.Domain.Providers;
using Orleans;
using Sketch.EventSourcing;

namespace CopilotTest1.People.Domain.Providers
{
    public interface IProviderAggregate : IGrainWithGuidKey
    {
        Task<ProviderState> GetState();

        Task ModifyHours(List<DayOfOperation> hours);

        /// <summary>
        /// Books a service appointment.
        /// </summary>
        /// <param name="locationServiceId"></param>
        /// <returns></returns>
        Task BookService(Guid locationServiceId);

        /// <summary>
        /// Cancels a service appointment.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        Task CancelService(ProviderProfile profile);
    }

    public class ProviderAggregate : Aggregate<ProviderState, ProviderDbContext>, IProviderAggregate
    {
        public ProviderAggregate(ProviderDbContext dbContext) : base(dbContext)
        {
        }

        public Task<ProviderState> GetState() => Task.FromResult(State);

        public async Task CancelService(ProviderProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            var existingProvider = await DbContext.GetProviderRefByEmail(profile.Email);

            if (existingProvider != null)
                throw new DomainException("Email in use.");

            RaiseDomainEvent<ProviderRegisteredEvent>((e) => { e.Profile = profile; });

            await ConfirmEvents();
        }

        public async Task ModifyProfile(ProviderProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            if (State.IsActive == false)
                throw new DomainException("Provider is not active.");

            var existingProvider = await DbContext.GetProviderRefByEmail(profile.Email);

            if (existingProvider != null && existingProvider.Id != this.GetPrimaryKey())
                throw new DomainException("Email in use.");

            RaiseDomainEvent<ProviderProfileModifiedEvent>((e) => { e.Profile = profile; });

            await ConfirmEvents();
        }

        public async Task Deactivate()
        {
            if (State.IsActive == false)
                return;

            RaiseDomainEvent<ProviderDeactivatedEvent>();

            await ConfirmEvents();
        }

        public async Task RequestAppointment(Guid locationServiceId)
        {
            if (State.Services.Any(i => i == locationServiceId))
                return;

            var locationServiceRef = await DbContext.GetLocationServiceRef(locationServiceId) ?? throw new DomainException("Service not found.");

            RaiseDomainEvent<ProviderServiceAddedEvent>((e) => { e.LocationServiceId = locationServiceId; });

            await ConfirmEvents();
        }

        public async Task BookService(Guid locationServiceId)
        {
            if (State.Services.All(i => i != locationServiceId))
                return;

            RaiseDomainEvent<ProviderServiceRemovedEvent>((e) => { e.LocationServiceId = locationServiceId; });

            await ConfirmEvents();
        }

        public async Task ModifyHours(List<DayOfOperation> hours)
        {
            if (hours == null)
                throw new ArgumentNullException(nameof(hours));

            if (hours.Count == 0)
                throw new DomainException("Hours cannot be empty.");

            if (hours.Any(i => i.Hours.Count == 0))
                throw new DomainException("Hours cannot be empty.");

            RaiseDomainEvent<ProviderHoursModifiedEvent>((e) => { e.Hours = hours; });

            await ConfirmEvents();
        }

        protected override async Task ApplyingUpdatesToStorage(IReadOnlyList<Event> updates)
        {
            foreach (var item in updates)
            {
                var existingRef = await DbContext.GetProviderRef(item.AggregateId);

                switch (item)
                {
                    case ProviderRegisteredEvent providerRegisteredEvent:

                        if (existingRef == null)
                        {
                            DbContext.ProviderRefs.Add(new ProviderRef
                            {
                                Id = providerRegisteredEvent.AggregateId,
                                Email = providerRegisteredEvent.Profile.Email
                            });
                        }

                        break;

                    case ProviderProfileModifiedEvent providerProfileModifiedEvent:

                        if (existingRef != null && existingRef.Email != providerProfileModifiedEvent.Profile.Email)
                        {
                            existingRef.Email = providerProfileModifiedEvent.Profile.Email;
                        }

                        break;
                }
            }
        }
    }
}
