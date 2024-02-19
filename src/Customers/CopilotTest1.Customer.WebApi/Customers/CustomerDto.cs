namespace CopilotTest1.People.WebApi.Customers
{
    public class CustomerDto
    {
        public CustomerProfileDto Profile { get; set; } = new CustomerProfileDto();

        public List<Guid> Providers { get; set; } = new List<Guid>();

        public bool IsActive { get; set; } = true;
    }
}
