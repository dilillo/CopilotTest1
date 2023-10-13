using ConsoleApp1.Provider;

namespace ConsoleApp1.Customer
{
    public class CustomerState
    {
        public CustomerProfile Profile { get; set; }

        public List<CustomerService> Services { get; set; } = new List<CustomerService>();

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

        public CustomerState Apply(CustomerServiceRequestedEvent customerPreferredServiceEvent)
        {
            Services.Add(customerPreferredServiceEvent.ProprietorLocationServiceID);

            return this;
        }

        public CustomerState Apply(CustomerServiceRemovedEvent customerUnpreferredServiceEvent)
        {
            Services.Remove(customerUnpreferredServiceEvent.ProprietorLocationServiceID);

            return this;
        }
    }
}
