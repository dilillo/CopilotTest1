using Orleans;
using Orleans.EventSourcing;

namespace ConsoleApp1.Appointment
{
    public interface IAppointmentAggregate : IGrainWithGuidKey
    {
        Task Request(AppointmentRequest appointmentBookingRequest);

        Task Confirm();

        Task Reject();

        Task Cancel();
    }

    public class AppointmentAggregate : JournaledGrain<AppointmentState>, IAppointmentAggregate
    {
        public async Task Request(AppointmentRequest appointmentBookingRequest)
        {
            if (appointmentBookingRequest.Start < DateTime.Now)
            {
                throw new AppointmentAlreadyHappenedException();
            }

            //validate available

            RaiseEvent(new AppointmentRequestedEvent
            {
                ProviderID = appointmentBookingRequest.ProviderID,
                CustomerID = appointmentBookingRequest.CustomerID,
                ProprietorLocationID = appointmentBookingRequest.ProprietorLocationID,
                ProprietorServiceID = appointmentBookingRequest.ProprietorServiceID,
                Start = appointmentBookingRequest.Start,
                End = appointmentBookingRequest.End
            });

            await ConfirmEvents();
        }

        public async Task Confirm()
        {
            RaiseEvent(new AppointmentConfirmedEvent());

            await ConfirmEvents();
        }

        public async Task Reject()
        {
            RaiseEvent(new AppointmentRejectedEvent());

            await ConfirmEvents();
        }

        public async Task Cancel()
        {
            RaiseEvent(new AppointmentCancelledEvent());

            await ConfirmEvents();
        }
    }
}
