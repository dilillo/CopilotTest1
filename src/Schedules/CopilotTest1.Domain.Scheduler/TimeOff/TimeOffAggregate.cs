using CopilotTest1.Shared.Data.Repositories;
using CopilotTest1.Shared.Domain.TimeOff;
using CopilotTest1.Shared.EventSourcing.Infrastructure;

namespace CopilotTest1.Scheduler.Domain.TimeOff
{
    public interface ITimeOffAggregate
    {
        Task Post(TimeOffRequest request);

        Task Cancel();
    }

    public class TimeOffAggregate : DomainAggregate<TimeOffState>, ITimeOffAggregate
    {
        public TimeOffAggregate(IEventRepository eventRepository, ISnapshotRepository snapshotRepository) : base(eventRepository, snapshotRepository)
        {
        }

        public async Task Post(TimeOffRequest request)
        {
            RaiseDomainEvent<TimeOffPostedEvent>((e) =>
            {
                e.Request = request;
            });

            await ConfirmEvents();
        }

        public async Task Cancel()
        {
            RaiseDomainEvent<TimeOffCancelledEvent>();

            await ConfirmEvents();
        }
    }
}
