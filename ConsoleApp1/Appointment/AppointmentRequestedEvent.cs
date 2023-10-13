namespace ConsoleApp1.Appointment
{
    public class AppointmentRequestedEvent
    {
        public Guid ProviderID { get; set; }

        public Guid CustomerID { get; set; }

        public Guid ProprietorLocationID { get; set; }

        public Guid ProprietorServiceID { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
