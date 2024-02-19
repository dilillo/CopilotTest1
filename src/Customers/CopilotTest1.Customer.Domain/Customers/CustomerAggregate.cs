using CopilotTest1.Customer.Data;
using CopilotTest1.Customer.Data.Entities;
using CopilotTest1.Shared.Domain.Appointments;
using CopilotTest1.Shared.Domain.Customers;
using CopilotTest1.Shared.Domain.Infrastructure;
using Orleans;
using Sketch.EventSourcing;

namespace CopilotTest1.Customer.Domain.Customers
{
    public interface ICustomerAggregate : IGrainWithGuidKey
    {
        Task<CustomerState> GetState();

        Task Register(Guid businessId, CustomerProfile profile);

        Task ModifyProfile(CustomerProfile profile);

        Task ReceiveService(AppointmentRequest appointment);
    }

    public class CustomerAggregate : Aggregate<CustomerState, CustomerDbContext>, ICustomerAggregate
    {
        public CustomerAggregate(CustomerDbContext dbContext) : base(dbContext)
        {
        }

        public Task<CustomerState> GetState() => Task.FromResult(State);

        public async Task Register(Guid businessId, CustomerProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));   
            
            var existingCustomer = await DbContext.GetCustomerRefByEmail(profile.Email);

            if (existingCustomer != null)
                throw new DomainException("Email in use.");

            RaiseDomainEvent<CustomerRegisteredEvent>((e) => { e.Profile = profile; });

            await ConfirmEvents();
        }

        public async Task ModifyProfile(CustomerProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            var existingCustomer = await DbContext.GetCustomerRefByEmail(profile.Email);
            
            if (existingCustomer != null && existingCustomer.Id != this.GetPrimaryKey())
                throw new DomainException("Email in use.");

            RaiseDomainEvent<CustomerProfileModifiedEvent>((e) => { e.Profile = profile; });

            await ConfirmEvents();
        }

        public Task ReceiveService(AppointmentRequest appointment) => throw new NotImplementedException();

        protected override async Task ApplyingUpdatesToStorage(IReadOnlyList<Event> updates)
        {
            foreach (var item in updates)
            {
                var existingRef = await DbContext.GetCustomerRef(item.AggregateId);

                switch (item)
                {
                    case CustomerRegisteredEvent customerRegisteredEvent:

                        if (existingRef == null)
                        {
                            DbContext.CustomerRefs.Add(new CustomerRef
                            {
                                Id = customerRegisteredEvent.AggregateId,
                                Email = customerRegisteredEvent.Profile.Email
                            });
                        }

                        break;

                    case CustomerProfileModifiedEvent customerProfileModifiedEvent:

                        if (existingRef != null && existingRef.Email != customerProfileModifiedEvent.Profile.Email)
                        {
                            existingRef.Email = customerProfileModifiedEvent.Profile.Email;
                        }

                        break;
                }
            }
        }

    }
}
