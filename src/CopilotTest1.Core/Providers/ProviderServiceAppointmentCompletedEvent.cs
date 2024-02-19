using CopilotTest1.Core.Infrastructure;

namespace CopilotTest1.Core.Domain.Providers
{
    public class ProviderServiceAppointmentCompletedEvent : DomainEvent
    {
        public Guid ProviderServiceAppointmentId { get; set; }
    }
}