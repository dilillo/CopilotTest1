using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.Core.Data.Entities
{
    public class ProviderRef
    {
        [Key]
        public Guid Id { get; set; }

        public ICollection<ProviderTimeOffRef> TimeOffs { get; } = new List<ProviderTimeOffRef>();

        public ICollection<LocationServiceProviderRef> Services { get; } = new List<LocationServiceProviderRef>();

        public ICollection<ProviderServiceAppointmentRef> Appointments { get; } = new List<ProviderServiceAppointmentRef>();

        public ICollection<ProviderHourRef> Hours { get; } = new List<ProviderHourRef>();
    }
}
