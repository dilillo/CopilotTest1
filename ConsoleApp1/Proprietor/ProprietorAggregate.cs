using Orleans.EventSourcing;

namespace ConsoleApp1.Proprietor
{
    public interface IProprietorAggregate
    {
        Task Register(ProprietorProfile profile);

        Task ModifyProfile(ProprietorProfile profile);

        Task Deactivate();
    }

    public class ProprietorAggregate : JournaledGrain<ProprietorState>, IProprietorAggregate
    {
        public async Task Register(ProprietorProfile profile)
        {
            RaiseEvent(new ProprietorRegisteredEvent
            {
                Profile = profile
            });

            await ConfirmEvents();
        }

        public async Task ModifyProfile(ProprietorProfile profile)
        {
            RaiseEvent(new ProprietorProfileModifiedEvent
            {
                Profile = profile
            });

            await ConfirmEvents();
        }

        public async Task Deactivate()
        {
            RaiseEvent(new ProprietorDeactivatedEvent());

            await ConfirmEvents();
        }
    }
}
