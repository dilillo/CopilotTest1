using CopilotTest1.Core.Data;
using CopilotTest1.Core.Data.Entities;
using CopilotTest1.Core.Infrastructure;
using CopilotTest1.Core.Providers;
using Orleans;
using Sketch.EventSourcing;

namespace CopilotTest1.Core.Domain.Providers
{
    public interface IProviderAggregate : IGrainWithGuidKey
    {
        Task<ProviderState> GetState();

        Task ModifyHours(List<ProviderLocationDayOfOperation> hours);

        Task BookAppointment(ProviderServiceAppointment appointment);

        Task CancelAppointment(Guid providerServiceAppointmentId);

        Task CompleteAppointment(Guid providerServiceAppointmentId);

        Task TakeTimeOff(ProviderTimeOff timeOff);

        Task CancelTimeOff(Guid providerTimeOffId);
    }

    public class ProviderAggregate : Aggregate<ProviderState, CoreDbContext>, IProviderAggregate
    {
        public ProviderAggregate(CoreDbContext dbContext) : base(dbContext)
        {
        }

        public Task<ProviderState> GetState() => Task.FromResult(State);

        public async Task BookAppointment(ProviderServiceAppointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            var providedService = await DbContext.GetLocationServiceProviderRef(appointment.LocationServiceId, this.GetPrimaryKey());

            if (providedService == null)
                throw new DomainException("Service not provided.");

            var locationService = await DbContext.GetLocationServiceRef(appointment.LocationServiceId);

            if (locationService == null)
                throw new DomainException("Service does not exist.");

            var appointmentDayOfTheWeek = appointment.Start.DayOfWeek;

            var providerLocationHoursForDay = State.Hours.Find(i => i.Day == appointmentDayOfTheWeek && i.LocationId == locationService.LocationId);

            if (providerLocationHoursForDay == null)
                throw new DomainException("Provider does not work on this day.");

            var appointmentIsOutsideOfWorkHours = !providerLocationHoursForDay.Hours.Any(i => i.StartTime <= appointment.Start.TimeOfDay
                && appointment.Start.AddMinutes(locationService.DurationInMinutes).TimeOfDay <=  i.EndTime);

            if (appointmentIsOutsideOfWorkHours)
                throw new DomainException("Provider does not work at this time.");

            var conflictExists = await DbContext.GetConflictingProviderServiceAppointmentExists(this.GetPrimaryKey(), appointment.LocationServiceId, appointment.Start, appointment.Start.AddMinutes(locationService.DurationInMinutes));

            if (conflictExists)
                throw new DomainException("Appointment conflicts with another appointment.");

            var outsideLocationHours = await DbContext.GetOutsideLocationHours(locationService.LocationId, appointment.Start, appointment.Start.AddMinutes(locationService.DurationInMinutes));

            if (outsideLocationHours)
                throw new DomainException("Appointment is outside of location hours.");

            var conflictsWithTimeOff = await DbContext.GetAppointmentConflictsWithProviderTimeOff(this.GetPrimaryKey(), appointment.Start, appointment.Start.AddMinutes(locationService.DurationInMinutes));

            if (conflictsWithTimeOff)
                throw new DomainException("Appointment conflicts with time off.");

            RaiseDomainEvent<ProviderServiceAppointmentBookedEvent>((e) =>
            {
                e.Appointment = appointment;
            });

            await ConfirmEvents();
        }

        public async Task CancelAppointment(Guid providerServiceAppointmentId)
        {
            var existingAppointment = await DbContext.GetProviderServiceAppointmentRef(providerServiceAppointmentId);

            if (existingAppointment == null)
                throw new DomainException("Appointment does not exist.");

            if (existingAppointment.ProviderId != this.GetPrimaryKey())
                throw new DomainException("Appointment does not belong to this provider.");

            if (existingAppointment.Start < DateTimeOffset.UtcNow)
                throw new DomainException("Appointment has already started.");

            RaiseDomainEvent<ProviderServiceAppointmentCancelledEvent>((e) =>
            {
                e.ProviderServiceAppointmentId = providerServiceAppointmentId;
            });

            await ConfirmEvents();
        }

        public async Task CompleteAppointment(Guid providerServiceAppointmentId)
        {
            var existingAppointment = await DbContext.GetProviderServiceAppointmentRef(providerServiceAppointmentId);

            if (existingAppointment == null)
                throw new DomainException("Appointment does not exist.");

            if (existingAppointment.ProviderId != this.GetPrimaryKey())
                throw new DomainException("Appointment does not belong to this provider.");

            RaiseDomainEvent<ProviderServiceAppointmentCompletedEvent>((e) =>
            {
                e.ProviderServiceAppointmentId = providerServiceAppointmentId;
            });

            await ConfirmEvents();
        }

        public async Task ModifyHours(List<ProviderLocationDayOfOperation> hours)
        {
            if (hours == null)
                throw new ArgumentNullException(nameof(hours));

            if (hours.Count == 0)
                throw new DomainException("Hours cannot be empty.");

            if (hours.Any(i => i.Hours.Count == 0))
                throw new DomainException("Hours cannot be empty.");

            var providerLocationHourGroups = hours.GroupBy(i => i.LocationId);

            foreach (var providerLocationHourGroup in providerLocationHourGroups)
            {
                var locationHours = await DbContext.GetLocationHourRefs(providerLocationHourGroup.Key);

                foreach (var providerLocationDay in providerLocationHourGroup)
                {
                    foreach (var providerLocationDayHour in providerLocationDay.Hours)
                    {
                        var locationHour = locationHours
                            .FirstOrDefault(i => i.Day == providerLocationDay.Day && i.StartTime <= providerLocationDayHour.StartTime && i.EndTime >= providerLocationDayHour.EndTime);

                        if (locationHour == null)
                            throw new DomainException("Location does not operate during this time.");
                    }
                }
            }

            RaiseDomainEvent<ProviderHoursModifiedEvent>((e) => { e.Hours = hours; });

            await ConfirmEvents();
        }

        public async Task TakeTimeOff(ProviderTimeOff timeOff)
        {
            if (timeOff == null)
                throw new ArgumentNullException(nameof(timeOff));

            var conflictingAppointmentsExist = await DbContext.GetAppointmentConflictsWithProviderTimeOff(this.GetPrimaryKey(), timeOff.Start, timeOff.End);

           if (conflictingAppointmentsExist)
                throw new DomainException("Time off conflicts with an existing appointment.");

            RaiseDomainEvent<ProviderTimeOffTakenEvent>((e) => { e.TimeOff = timeOff; });

            await ConfirmEvents();
        }

        public async Task CancelTimeOff(Guid providerTimeOffId)
        {
            var existingTimeOff = await DbContext.GetProviderTimeOffRef(providerTimeOffId);

            if (existingTimeOff == null)
                throw new DomainException("Time off does not exist.");

            if (existingTimeOff.ProviderId != this.GetPrimaryKey())
                throw new DomainException("Time off does not belong to this provider.");

            RaiseDomainEvent<ProviderTimeOffCancelledEvent>((e) => { e.ProviderTimeOffId = providerTimeOffId; });

            await ConfirmEvents(); 
        }

        protected override async Task ApplyingUpdatesToStorage(IReadOnlyList<Event> updates)
        {
            foreach (var item in updates)
            {
                switch (item)
                {
                   case ProviderHoursModifiedEvent providerHoursModifiedEvent:

                        await UpdateProviderLocationHourRefs(this.GetPrimaryKey(), providerHoursModifiedEvent.Hours);

                        break;

                    case ProviderServiceAppointmentBookedEvent providerServiceAppointmentBookedEvent:

                        if (providerServiceAppointmentBookedEvent.Appointment == null)
                            throw new ArgumentNullException(nameof(providerServiceAppointmentBookedEvent.Appointment));

                        DbContext.ProviderServiceAppointmentRefs.Add(new ProviderServiceAppointmentRef
                        {
                            Id = providerServiceAppointmentBookedEvent.Appointment.Id,
                            ProviderId = this.GetPrimaryKey(),
                            LocationServiceId = providerServiceAppointmentBookedEvent.Appointment.LocationServiceId,
                            Start = providerServiceAppointmentBookedEvent.Appointment.Start,
                            CustomerId = providerServiceAppointmentBookedEvent.Appointment.CustomerId
                        });

                        break;

                    case ProviderServiceAppointmentCancelledEvent providerServiceAppointmentCancelledEvent:
                        
                        var existingAppointment = await DbContext.GetProviderServiceAppointmentRef(providerServiceAppointmentCancelledEvent.ProviderServiceAppointmentId);

                        if (existingAppointment == null)
                            throw new DomainException("Appointment does not exist.");

                        DbContext.Remove(existingAppointment);

                        break;

                    case ProviderTimeOffTakenEvent providerTimeOffTakenEvent:

                        if (providerTimeOffTakenEvent.TimeOff == null)
                            throw new ArgumentNullException(nameof(providerTimeOffTakenEvent.TimeOff));

                        DbContext.ProviderTimeOffRefs.Add(new ProviderTimeOffRef
                        {
                            Id = providerTimeOffTakenEvent.TimeOff.Id,
                            ProviderId = this.GetPrimaryKey(),
                            Start = providerTimeOffTakenEvent.TimeOff.Start,
                            End = providerTimeOffTakenEvent.TimeOff.End
                        });

                        await DbContext.AddAsync(providerTimeOffTakenEvent.TimeOff);

                        break;

                    case ProviderTimeOffCancelledEvent providerTimeOffCancelledEvent:
                        
                        var existingTimeOff = await DbContext.GetProviderTimeOffRef(providerTimeOffCancelledEvent.ProviderTimeOffId);

                        if (existingTimeOff == null)
                            throw new DomainException("Time off does not exist.");

                        DbContext.Remove(existingTimeOff);

                        break;
                }
            }
        }

        private async Task UpdateProviderLocationHourRefs(Guid providerId, List<ProviderLocationDayOfOperation> dayOfOperations)
        {
            var existingRefs = await DbContext.GetProviderHourRefs(providerId);

            foreach (var dayOfOperation in dayOfOperations)
            {
                foreach (var periodOfOperation in dayOfOperation.Hours)
                {
                    var existingRef = existingRefs.FirstOrDefault(i => i.LocationId == dayOfOperation.LocationId && i.Day == dayOfOperation.Day && i.StartTime == periodOfOperation.StartTime && i.EndTime == periodOfOperation.EndTime);

                    if (existingRef == null)
                    {
                        existingRef = new ProviderHourRef
                        {
                            ProviderId = providerId,
                            LocationId = dayOfOperation.LocationId,
                            Day = dayOfOperation.Day,
                            StartHour = periodOfOperation.StartHour,
                            StartMinute = periodOfOperation.StartMinute,
                            EndHour = periodOfOperation.EndHour,
                            EndMinute = periodOfOperation.EndMinute
                        };

                        DbContext.ProviderHourRefs.Add(existingRef);
                    }
                }
            }

            foreach (var existingRef in existingRefs)
            {
                var dailyLocationHours = dayOfOperations
                    .Where(i => i.LocationId == existingRef.LocationId && i.Day == existingRef.Day)
                    .SelectMany(i => i.Hours)
                    .ToArray();

                if (!dailyLocationHours.Any(i => i.StartTime == existingRef.StartTime && i.EndTime == existingRef.EndTime))
                {
                    DbContext.ProviderHourRefs.Remove(existingRef);
                }
            }
        }
    }
}
