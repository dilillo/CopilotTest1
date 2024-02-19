using CopilotTest1.Core.Infrastructure;
using CopilotTest1.Core.Providers;

namespace CopilotTest1.Core.Domain.Providers
{
    public class ProviderServiceAppointmentBookedEvent : DomainEvent
    {
        public ProviderServiceAppointment? Appointment { get; set; }
    }
}