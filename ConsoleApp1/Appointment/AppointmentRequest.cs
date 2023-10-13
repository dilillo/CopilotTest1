namespace ConsoleApp1.Appointment
{
    public class AppointmentRequest
    {
        public Guid ProviderID { get; set; }

        public Guid CustomerID { get; set; }

        public Guid ProprietorLocationID { get; set; }

        public Guid ProprietorServiceID { get; set; }

        public DateTime Start { get; set; }

        public int DurationInMinutes { get; set; }
    }
}
