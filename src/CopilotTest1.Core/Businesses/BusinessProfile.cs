namespace CopilotTest1.Core.Businesses
{
    [GenerateSerializer]
    public class BusinessProfile
    {
        [Id(0)]
        public string Name { get; set; } = string.Empty;

        [Id(1)]
        public string Description { get; set; } = string.Empty;

        [Id(2)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
