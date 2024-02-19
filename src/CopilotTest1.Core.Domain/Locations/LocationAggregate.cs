using CopilotTest1.Core.Data;
using CopilotTest1.Core.Data.Entities;
using CopilotTest1.Core.Infrastructure;
using CopilotTest1.Core.Locations;
using Orleans;
using Sketch.EventSourcing;

namespace CopilotTest1.Core.Domain.Locations
{
    public interface ILocationAggregate : IGrainWithGuidKey
    {
        Task<LocationState> GetState();

        Task Open(Guid businessId, LocationProfile profile, List<DayOfOperation> hours);

        Task PermanentlyClose();

        Task ModifyHours(List<DayOfOperation> hours);

        Task ModifyProfile(LocationProfile profile);

        Task AddService(LocationService locationService);

        Task ModifyService(LocationService locationService);

        Task RemoveService(Guid locationServiceId);

        Task AddServiceProvider(LocationServiceProvider locationServiceProvider);

        Task RemoveServiceProvider(LocationServiceProvider locationServiceProvider);

        Task AddCustomer(Guid customerId);

        Task RemoveCustomer(Guid customerId);

        Task AddClosing(LocationClosing closing);

        Task RemoveClosing(Guid locationClosingId);
    }

    public class LocationAggregate : Aggregate<LocationState, CoreDbContext>, ILocationAggregate
    {
        public LocationAggregate(CoreDbContext dbContext) : base(dbContext)
        {
        }

        public Task<LocationState> GetState() => Task.FromResult(State);

        public async Task Open(Guid businessId, LocationProfile profile, List<DayOfOperation> hours)
        {
            if (profile == null) 
                throw new ArgumentNullException(nameof(profile));

            if (profile.Address == null)
                throw new ArgumentNullException(nameof(profile));

            if (hours == null) 
                throw new ArgumentNullException(nameof(hours));

            if (hours.Count == 0) 
                throw new DomainException("Hours cannot be empty.");

            var business = await DbContext.GetBusinessRef(businessId);

            if (business == null)
                throw new DomainException("Business does not exist.");

            if (business.IsPermanentlyClosed)
                throw new DomainException("Business is permanently closed.");

            var existingLocation = await DbContext.GetLocationRefByName(businessId, profile.Name);

            if (existingLocation != null)
                throw new DomainException("Location name already exists.");

            RaiseDomainEvent<LocationOpenedEvent>((e) =>
            {
                e.BusinessId = businessId;
                e.Profile = profile;
                e.Hours = hours;
            });

            await ConfirmEvents();
        }

        public async Task ModifyProfile(LocationProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            if (profile.Address == null)
                throw new ArgumentNullException(nameof(profile));

            RaiseDomainEvent<LocationProfileModifiedEvent>((e) =>
            {
                e.Profile = profile;
            });

            await ConfirmEvents();
        }

        public async Task ModifyHours(List<DayOfOperation> hours)
        {
            if (hours == null)
                throw new ArgumentNullException(nameof(hours));

            if (hours.Count == 0)
                throw new DomainException("Hours cannot be empty.");

            RaiseDomainEvent<LocationHoursModifiedEvent>((e) =>
            {
                e.Hours = hours;
            });

            await ConfirmEvents();
        }

        public async Task AddService(LocationService locationService)
        {
            if (locationService == null)
                throw new ArgumentNullException(nameof(locationService));

            var existingService = State.Services.FirstOrDefault(i => i.Name == locationService.Name);

            if (existingService != null)
                throw new DomainException("Service already exists.");

            RaiseDomainEvent<LocationServiceAddedEvent>((e) =>
            {
                e.Service = locationService;
            });

            await ConfirmEvents();
        }

        public async Task ModifyService(LocationService locationService)
        {
            if (locationService == null)
                throw new ArgumentNullException(nameof(locationService));

            var existingService = State.Services.FirstOrDefault(i => i.Name == locationService.Name && i.Id != locationService.Id);

            if (existingService != null)
                throw new DomainException("Service already exists.");

            RaiseDomainEvent<LocationServiceModifiedEvent>((e) =>
            {
                e.Service = locationService;
            });

            await ConfirmEvents();
        }

        public async Task RemoveService(Guid locationServiceId)
        {
            if (State.Services.All(i => i.Id != locationServiceId))
                return;

            RaiseDomainEvent<LocationServiceRemovedEvent>((e) =>
            {
                e.LocationServiceId = locationServiceId;
            });

            await ConfirmEvents();
        }

        public async Task AddServiceProvider(LocationServiceProvider locationServiceProvider)
        {
            if (locationServiceProvider == null)
                throw new ArgumentNullException(nameof(locationServiceProvider));

            var existingServiceProvider = State.ServiceProviders.FirstOrDefault(i => i.ProviderId == locationServiceProvider.ProviderId && i.LocationServiceId == locationServiceProvider.LocationServiceId);

            if (existingServiceProvider != null)
                throw new DomainException("Service provider already exists.");

            var existingService = State.Services.FirstOrDefault(i => i.Id == locationServiceProvider.LocationServiceId);

            if (existingService == null)
                throw new DomainException("Service does not exist.");

            var existingProvider = await DbContext.GetProviderRef(locationServiceProvider.ProviderId);

            if (existingProvider == null)
                throw new DomainException("Provider does not exist.");

            RaiseDomainEvent<LocationServiceProviderAddedEvent>((e) =>
            {
                e.ServiceProvider = locationServiceProvider;
            });

            await ConfirmEvents();
        }

        public async Task RemoveServiceProvider(LocationServiceProvider locationServiceProvider)
        {
            if (locationServiceProvider == null)
                throw new ArgumentNullException(nameof(locationServiceProvider));

            if (State.ServiceProviders.All(i => i.ProviderId != locationServiceProvider.ProviderId && i.LocationServiceId != locationServiceProvider.LocationServiceId))
                return;

            RaiseDomainEvent<LocationServiceProviderRemovedEvent>((e) =>
            {
                e.ServiceProvider = locationServiceProvider;
            });

            await ConfirmEvents();
        }

        public async Task AddCustomer(Guid customerId)
        {
            RaiseDomainEvent<LocationCustomerAddedEvent>((e) =>
            {
                e.CustomerId = customerId;
            });

            await ConfirmEvents();
        }

        public async Task RemoveCustomer(Guid customerId)
        {
            RaiseDomainEvent<LocationCustomerRemovedEvent>((e) =>
            {
                e.CustomerId = customerId;
            });

            await ConfirmEvents();
        }

        public async Task PermanentlyClose()
        {
            if (State.IsPermanentlyClosed)
                return;

            RaiseDomainEvent<LocationPermanentlyClosedEvent>();

            await ConfirmEvents();
        }

        public async Task AddClosing(LocationClosing closing)
        {
            if (closing == null)
                throw new ArgumentNullException(nameof(closing));

            var startDayOfWeekHours = State.Hours.FirstOrDefault(i => i.Day == closing.Start.DayOfWeek);

            if (startDayOfWeekHours == null)
                throw new DomainException("Closing start day of week hours do not exist.");

            var startDayOfWeekHoursFirstPeriod = startDayOfWeekHours.Hours.FirstOrDefault();

            if (startDayOfWeekHoursFirstPeriod == null)
                throw new DomainException("Closing start day of week hours do not exist.");

            if (startDayOfWeekHoursFirstPeriod.StartTime > closing.Start.TimeOfDay)
                throw new DomainException("Closing start time is outside of hours.");

            var endDayOfWeekHours = State.Hours.FirstOrDefault(i => i.Day == closing.End.DayOfWeek);

            if (endDayOfWeekHours == null)
                throw new DomainException("Closing end day of week hours do not exist.");
            
            var endDayOfWeekHoursLastPeriod = endDayOfWeekHours.Hours.LastOrDefault();

            if (endDayOfWeekHoursLastPeriod == null)
                throw new DomainException("Closing end day of week hours do not exist.");

            if (endDayOfWeekHoursLastPeriod.EndMinute < closing.End.Minute)
                throw new DomainException("Closing end time is outside of hours.");

            var hasAppointmentConflicts = await DbContext.GetLocationClosingHasAppointmentConflicts(this.GetPrimaryKey(), closing.Start, closing.End);

            if (hasAppointmentConflicts)
                throw new DomainException("Closing has appointment conflicts.");

            RaiseDomainEvent<LocationClosingAddedEvent>((e) => e.Closing = closing);

            await ConfirmEvents();
        }

        public async Task RemoveClosing(Guid locationClosingId)
        {
            var existingClosing = await DbContext.GetLocationClosingRef(locationClosingId);

            if (existingClosing == null)
                return; 

            if (existingClosing.LocationId != this.GetPrimaryKey())
                throw new DomainException("Closing does not belong to this location.");

            RaiseDomainEvent<LocationClosingRemovedEvent>((e) => e.LocationClosingId = locationClosingId);

            await ConfirmEvents();
        }

        protected override async Task ApplyingUpdatesToStorage(IReadOnlyList<Event> updates)
        {
            foreach (var item in updates)
            {
                switch (item)
                {
                    case LocationOpenedEvent locationOpenedEvent:

                        await UpdateLocationRef(this.GetPrimaryKey(), locationRef =>
                        {
                            locationRef.Name = locationOpenedEvent.Profile?.Name ?? string.Empty;
                            locationRef.BusinessId = locationOpenedEvent.BusinessId;
                        });

                        break;

                    case LocationProfileModifiedEvent locaitonProfileModifiedEvent:

                        await UpdateLocationRef(this.GetPrimaryKey(), locationRef => locationRef.Name = locaitonProfileModifiedEvent.Profile?.Name ?? string.Empty);

                        break;

                    case LocationPermanentlyClosedEvent:

                        await UpdateLocationRef(this.GetPrimaryKey(), locationRef => locationRef.IsPermanentlyClosed = true);

                        break;

                    case LocationHoursModifiedEvent locationHoursModifiedEvent:

                        await UpdateLocationHourRefs(this.GetPrimaryKey(), locationHoursModifiedEvent.Hours);

                        break;

                    case LocationClosingAddedEvent locationClosingAddedEvent:

                        await UpdateLocationClosingRef(locationClosingAddedEvent.Id, this.GetPrimaryKey(), locationClosingRef =>
                        {
                            if (locationClosingAddedEvent.Closing == null)
                                throw new ArgumentNullException(nameof(locationClosingAddedEvent.Closing));

                            locationClosingRef.Start = locationClosingAddedEvent.Closing.Start;
                            locationClosingRef.End = locationClosingAddedEvent.Closing.End;
                        });

                        break;

                    case LocationClosingRemovedEvent locationClosingRemovedEvent:

                        var locationClosingToRemove = await DbContext.GetLocationClosingRef(locationClosingRemovedEvent.LocationClosingId);

                        if (locationClosingToRemove == null)
                            break;

                        DbContext.LocationClosingRefs.Remove(locationClosingToRemove);

                        break;

                    case LocationServiceProviderAddedEvent locationServiceProviderAddedEvent:

                        if (locationServiceProviderAddedEvent.ServiceProvider == null)
                            throw new ArgumentNullException(nameof(locationServiceProviderAddedEvent.ServiceProvider));

                        await UpdateLocationServiceProviderRef(locationServiceProviderAddedEvent.ServiceProvider.LocationServiceId, locationServiceProviderAddedEvent.ServiceProvider.ProviderId, locationServiceProviderRef => { });

                        break;

                    case LocationServiceProviderRemovedEvent locationServiceProviderRemovedEvent:

                        if (locationServiceProviderRemovedEvent.ServiceProvider == null)
                            throw new ArgumentNullException(nameof(locationServiceProviderRemovedEvent.ServiceProvider));

                        var locationServiceProviderToRemove = await DbContext.GetLocationServiceProviderRef(locationServiceProviderRemovedEvent.ServiceProvider.LocationServiceId, locationServiceProviderRemovedEvent.ServiceProvider.ProviderId);

                        if (locationServiceProviderToRemove == null)
                            break;

                        DbContext.LocationServiceProviderRefs.Remove(locationServiceProviderToRemove);

                        break;

                    case LocationServiceAddedEvent locationServiceAddedEvent:

                        await UpdateLocationServiceRef(locationServiceAddedEvent.Id, this.GetPrimaryKey(), locationServiceRef =>
                        {
                            if (locationServiceAddedEvent.Service == null)
                                throw new ArgumentNullException(nameof(locationServiceAddedEvent.Service));

                            locationServiceRef.DurationInMinutes = locationServiceAddedEvent.Service.DurationInMinutes;
                        });

                        break;

                    case LocationServiceModifiedEvent locationServiceModifiedEvent:

                        await UpdateLocationServiceRef(locationServiceModifiedEvent.Id, this.GetPrimaryKey(), locationServiceRef =>
                        {
                            if (locationServiceModifiedEvent.Service == null)
                                throw new ArgumentNullException(nameof(locationServiceModifiedEvent.Service));

                            locationServiceRef.DurationInMinutes = locationServiceModifiedEvent.Service.DurationInMinutes;
                        });

                        break;

                    case LocationServiceRemovedEvent locationServiceRemovedEvent:

                        var locationServiceToRemove = await DbContext.GetLocationServiceRef(locationServiceRemovedEvent.LocationServiceId);

                        if (locationServiceToRemove == null) 
                            break;

                        DbContext.LocationServiceRefs.Remove(locationServiceToRemove);

                        break;

                }
            }
        }

        private async Task UpdateLocationRef(Guid id, Action<LocationRef> update)
        {
            var existingRef = await DbContext.GetLocationRef(id);

            if (existingRef == null)
            {
                existingRef = new LocationRef
                {
                    Id = id,
                };

                DbContext.LocationRefs.Add(existingRef);
            }

            update(existingRef);
        }

        private async Task UpdateLocationServiceRef(Guid id, Guid locationId, Action<LocationServiceRef> update)
        {
            var existingRef = await DbContext.GetLocationServiceRef(id);

            if (existingRef == null)
            {
                existingRef = new LocationServiceRef
                {
                    Id = id,
                    LocationId = locationId
                };

                DbContext.LocationServiceRefs.Add(existingRef);
            }

            update(existingRef);
        }

        private async Task UpdateLocationServiceProviderRef(Guid locationServiceId, Guid providerId, Action<LocationServiceProviderRef> update)
        {
            var existingRef = await DbContext.GetLocationServiceProviderRef(locationServiceId, providerId);

            if (existingRef == null)
            {
                existingRef = new LocationServiceProviderRef
                {
                    LocationServiceId = locationServiceId, 
                    ProviderId = providerId
                };

                DbContext.LocationServiceProviderRefs.Add(existingRef);
            }

            update(existingRef);
        }

        private async Task UpdateLocationClosingRef(Guid id, Guid locationId, Action<LocationClosingRef> update)
        {
            var existingRef = await DbContext.GetLocationClosingRef(id);

            if (existingRef == null)
            {
                existingRef = new LocationClosingRef
                {
                     Id = id,
                     LocationId = locationId
                };

                DbContext.LocationClosingRefs.Add(existingRef);
            }

            update(existingRef);
        }

        private async Task UpdateLocationHourRefs(Guid locationId, List<DayOfOperation> dayOfOperations)
        {
            var existingRefs = await DbContext.GetLocationHourRefs(locationId);

            foreach (var dayOfOperation in dayOfOperations)
            {
                foreach (var periodOfOperation in dayOfOperation.Hours)
                {
                    var existingRef = existingRefs.FirstOrDefault(i => i.Day == dayOfOperation.Day && i.StartTime == periodOfOperation.StartTime && i.EndTime == periodOfOperation.EndTime);

                    if (existingRef == null)
                    {
                        existingRef = new LocationHourRef
                        {
                            LocationId = locationId,
                            Day = dayOfOperation.Day,
                            StartHour = periodOfOperation.StartHour,
                            StartMinute = periodOfOperation.StartMinute,
                            EndHour = periodOfOperation.EndHour,
                            EndMinute = periodOfOperation.EndMinute
                        };

                        DbContext.LocationHourRefs.Add(existingRef);
                    }
                }   
            }   

            foreach (var existingRef in existingRefs)
            {
                var dailyHours = dayOfOperations.Where(i => i.Day == existingRef.Day).SelectMany(i => i.Hours).ToArray();

                if (!dailyHours.Any(i => i.StartTime == existingRef.StartTime && i.EndTime == existingRef.EndTime))
                {
                    DbContext.LocationHourRefs.Remove(existingRef);
                }
            }
        }
    }
}
