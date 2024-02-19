using CopilotTest1.Shared.Domain.Appointments;

namespace CopilotTest1.Scheduler.Domain.Appointments
{
    public class AppointmentState
    {
        public Guid ProviderId { get; set; }

        public Guid CustomerId { get; set; }

        public Guid LocationServiceId { get; set; }

        public DateTime Start { get; set; }

        public bool IsAccepted { get; set; } = false;

        public bool IsRejected { get; set; } = false;

        public bool IsCancelled { get; set; } = false;

        public AppointmentState Apply(AppointmentRequestedEvent appointmentRequestedEvent)
        {
            ProviderId = appointmentRequestedEvent.ProviderId;
            CustomerId = appointmentRequestedEvent.CustomerId;
            LocationServiceId = appointmentRequestedEvent.LocationServiceId;
            Start = appointmentRequestedEvent.Start;

            return this;
        }

        public AppointmentState Apply(AppointmentConfirmedEvent appointmentAcceptedEvent)
        {
            IsAccepted = true;

            return this;
        }

        public AppointmentState Apply(AppointmentRejectedEvent appointmentRejectedEvent)
        {
            IsRejected = true;

            return this;
        }

        public AppointmentState Apply(AppointmentCancelledEvent appointmentCancelledEvent)
        {
            IsCancelled = true;

            return this;
        }
    }
}
