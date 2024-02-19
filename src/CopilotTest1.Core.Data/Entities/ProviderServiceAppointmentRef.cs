using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.Core.Data.Entities
{
    public class ProviderServiceAppointmentRef
    {
        [Key]
        public Guid Id { get; set; }

        public Guid LocationServiceId { get; set; }

        public LocationServiceRef LocationService { get; set; } = null!;

        public Guid ProviderId { get; set; }

        public ProviderRef Provider { get; set; } = null!;

        public Guid? CustomerId { get; set; }

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }
    }
}
