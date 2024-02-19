using CopilotTest1.Shared.Data.Repositories;
using CopilotTest1.Shared.Domain.Closings;
using CopilotTest1.Shared.EventSourcing.Infrastructure;

namespace CopilotTest1.Scheduler.Domain.Closings
{
    public interface IClosingAggregate
    {
        Task Post(ClosingRequest request);

        Task Cancel();
    }

    public class ClosingAggregate : DomainAggregate<ClosingState>, IClosingAggregate
    {
        public ClosingAggregate(IEventRepository eventRepository, ISnapshotRepository snapshotRepository) : base(eventRepository, snapshotRepository)
        {
        }

        public async Task Post(ClosingRequest request)
        {
            RaiseDomainEvent<ClosingPostedEvent>((e) =>
            {
                e.Request = request;
            });

            await ConfirmEvents();
        }

        public async Task Cancel()
        {
            RaiseDomainEvent<ClosingCancelledEvent>();

            await ConfirmEvents();
        }
    }
}
