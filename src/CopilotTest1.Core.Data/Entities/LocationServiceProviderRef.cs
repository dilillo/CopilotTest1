namespace CopilotTest1.Core.Data.Entities
{
    public class LocationServiceProviderRef
    {
        public Guid LocationServiceId { get; set; }

        public LocationServiceRef LocationService { get; set; } = null!;

        public Guid ProviderId { get; set; }

        public ProviderRef Provider { get; set; } = null!;

        public ICollection<ProviderServiceAppointmentRef> ProviderServiceAppointments { get; } = new List<ProviderServiceAppointmentRef>();
    }
}
