namespace CopilotTest1.Core.Providers
{
    [GenerateSerializer]
    public class ProviderTimeOff
    {
        [Id(0)]
        public Guid Id { get; set; }

        [Id(1)]
        public DateTimeOffset Start { get; set; }

        [Id(2)]
        public DateTimeOffset End { get; set; }

        [Id(3)]
        public string Reason { get; set; } = string.Empty;
    }
}
