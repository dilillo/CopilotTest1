namespace ConsoleApp1.Appointment
{
    public class AppointmentState
    {
        public Guid ProviderID { get; set; }

        public Guid CustomerID { get; set; }

        public Guid ProprietorLocationID { get; set; }

        public Guid ProprietorServiceID { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsRejected { get; set; }

        public bool IsCancelled { get; set; }

        public AppointmentState Apply(AppointmentRequestedEvent appointmentBookedEvent)
        {
            ProviderID = appointmentBookedEvent.ProviderID;
            CustomerID = appointmentBookedEvent.CustomerID;
            ProprietorLocationID = appointmentBookedEvent.ProprietorLocationID;
            ProprietorServiceID = appointmentBookedEvent.ProprietorServiceID;
            Start = appointmentBookedEvent.Start;
            End = appointmentBookedEvent.End;

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
