using CopilotTest1.Shared.Data.Repositories;
using CopilotTest1.Shared.Domain.Appointments;
using CopilotTest1.Shared.EventSourcing.Infrastructure;
using Orleans;

namespace CopilotTest1.Scheduler.Domain.Appointments
{
    public interface IAppointmentAggregate : IGrainWithGuidKey
    {
        Task Request(AppointmentRequest appointmentBookingRequest);

        Task Confirm();

        Task Reject();

        Task Cancel();
    }

    public class AppointmentAggregate : DomainAggregate<AppointmentState>, IAppointmentAggregate
    {
        public AppointmentAggregate(IEventRepository eventRepository, ISnapshotRepository snapshotRepository) : base(eventRepository, snapshotRepository)
        {
        }

        public async Task Request(AppointmentRequest appointmentRequest)
        {
            RaiseDomainEvent<AppointmentRequestedEvent>((e) =>
            {
                e.ProviderId = appointmentRequest.ProviderId;
                e.CustomerId = appointmentRequest.CustomerId;
                e.LocationServiceId = appointmentRequest.LocationServiceId;
                e.Start = appointmentRequest.Start;
            });

            await ConfirmEvents();
        }

        public async Task Confirm()
        {
            RaiseDomainEvent<AppointmentConfirmedEvent>();

            await ConfirmEvents();
        }

        public async Task Reject()
        {
            RaiseDomainEvent<AppointmentRejectedEvent>();

            await ConfirmEvents();
        }

        public async Task Cancel()
        {
            RaiseDomainEvent<AppointmentCancelledEvent>();

            await ConfirmEvents();
        }
    }
}
