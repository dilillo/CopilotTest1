using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Domain.Providers
{
    public class ProviderServiceAppointmentCancelledEvent : DomainEvent
    {
        public Guid ProviderServiceAppointmentId { get; set; }
    }
}