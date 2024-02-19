using CopilotTest1.Shared.Domain.Customers;

namespace CopilotTest1.Customer.Domain.Customers
{
    public class CustomerState
    {
        public CustomerProfile Profile { get; set; } = new CustomerProfile();

        public List<Guid> Relationships { get; set; } = new List<Guid>();

        public bool IsActive { get; set; } = true;

        public CustomerState Apply(CustomerRegisteredEvent customerRegisteredEvent)
        {
            Profile = customerRegisteredEvent.Profile;
            return this;
        }

        public CustomerState Apply(CustomerProfileModifiedEvent cusotmerProfileModifiedEvent)
        {
            Profile = cusotmerProfileModifiedEvent.Profile;

            return this;
        }
    }
}
