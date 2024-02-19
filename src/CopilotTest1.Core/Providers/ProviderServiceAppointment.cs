namespace CopilotTest1.Core.Providers
{
    [GenerateSerializer]
    public class ProviderServiceAppointment
    {
        [Id(0)]
        public Guid Id { get; set; }

        [Id(1)]
        public Guid? CustomerId { get; set; }

        [Id(2)]
        public Guid LocationServiceId { get; set; }

        [Id(3)]
        public DateTimeOffset Start { get; set; }
    }
}
