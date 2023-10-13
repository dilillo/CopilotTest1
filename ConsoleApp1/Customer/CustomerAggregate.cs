using ConsoleApp1.Appointment;
using ConsoleApp1.Provider;
using Orleans;
using Orleans.EventSourcing;

namespace ConsoleApp1.Customer
{
    public interface ICustomerAggregate : IGrainWithGuidKey
    {
        Task Register(CustomerProfile customerProfile);

        Task ModifyProfile(CustomerProfile customerProfile);

        Task RequestService(Guid proprietorLocationServiceID);

        Task AcceptService(Guid proprietorLocationServiceID);

        Task RejectService(Guid proprietorLocationServiceID, string reason);

        Task SuspendService(Guid proprietorLocationServiceID, string reason);

        Task ReinstateService(Guid proprietorLocationServiceID);

        Task RemoveService(Guid proprietorLocationServiceID);
    }

    public class CustomerAggregate : JournaledGrain<AppointmentState>, ICustomerAggregate
    {
        public async Task Register(CustomerProfile customerProfile)
        {
            RaiseEvent(new CustomerRegisteredEvent { Profile = customerProfile });

            await ConfirmEvents();
        }

        public async Task ModifyProfile(CustomerProfile customerProfile)
        {
            RaiseEvent(new CustomerProfileModifiedEvent { Profile = customerProfile });

            await ConfirmEvents();
        }

        public async Task RequestService(Guid proprietorLocationServiceID)
        {
            RaiseEvent(new CustomerServiceRequestedEvent { ProprietorLocationServiceID = proprietorLocationServiceID });

            await ConfirmEvents();
        }

        public async Task AcceptService(Guid proprietorLocationServiceID)
        {
            RaiseEvent(new CustomerServiceAcceptedEvent { ProprietorLocationServiceID = proprietorLocationServiceID });

            await ConfirmEvents();
        }

        public async Task RejectService(Guid proprietorLocationServiceID, string reason)
        {
            RaiseEvent(new CustomerServiceRejectedEvent { ProprietorLocationServiceID = proprietorLocationServiceID, Reason = reason });

            await ConfirmEvents();
        }

        public async Task SuspendService(Guid proprietorLocationServiceID, string reason)
        {
            RaiseEvent(new CustomerServiceSuspendedEvent { ProprietorLocationServiceID = proprietorLocationServiceID, Reason = reason});

            await ConfirmEvents();
        }

        public async Task ReinstateService(Guid proprietorLocationServiceID)
        {
            RaiseEvent(new CustomerServiceReinstatedEvent { ProprietorLocationServiceID = proprietorLocationServiceID });

            await ConfirmEvents();
        }

        public async Task RemoveService(Guid proprietorLocationServiceID)
        {
            RaiseEvent(new CustomerServiceRemovedEvent { ProprietorLocationServiceID = proprietorLocationServiceID });

            await ConfirmEvents();
        }
    }
}
