using ConsoleApp1.Infrastructure;
using Orleans.EventSourcing;

namespace ConsoleApp1.Location
{
    public class LocationAggregate : JournaledGrain<LocationState>
    {
        public async Task Open(LocationProfile profile)
        {
            RaiseEvent(new LocationOpenedEvent
            {
                Profile = profile
            });

            await ConfirmEvents();
        }

        public async Task ModifyProfile(LocationProfile profile)
        {
            RaiseEvent(new LocationProfileModifiedEvent
            {
                Profile = profile
            });

            await ConfirmEvents();
        }

        public async Task ModifyAddress(LocationAddress address)
        {
            RaiseEvent(new LocationAddressModifiedEvent
            {
                Address = address
            });

            await ConfirmEvents();
        }

        public async Task ModifyHours(List<DayOfOperation> hours)
        {
            RaiseEvent(new LocationHoursModifiedEvent
            {
                Hours = hours
            });

            await ConfirmEvents();
        }

        public async Task OfferService(LocationService service)
        {
            RaiseEvent(new LocationServiceOfferredEvent
            {
                Service = service
            });

            await ConfirmEvents();
        }

        public async Task ModifyService(LocationService service)
        {
            RaiseEvent(new LocationServiceModifiedEvent
            {
                Service = service
            });

            await ConfirmEvents();
        }

        public async Task SuspendService(Guid locationServiceID)
        {
            RaiseEvent(new LocationServiceSuspendedEvent
            {
                LocationServiceID = locationServiceID
            });

            await ConfirmEvents();
        }
    }
}
