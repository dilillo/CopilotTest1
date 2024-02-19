using CopilotTest1.Shared.Domain.Closings;

namespace CopilotTest1.Scheduler.Domain.Closings
{
    public class ClosingState
    {
        public Guid LocationId { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        public string Reason { get; set; } = string.Empty;

        public bool IsCancelled { get; set; }

        public ClosingState Apply(ClosingPostedEvent closingPostedEvent)
        {
            LocationId = closingPostedEvent.Request.LocationId;
            Start = closingPostedEvent.Request.Start;
            End = closingPostedEvent.Request.End;
            Reason = closingPostedEvent.Request.Reason;

            return this;
        }

        public ClosingState Apply(ClosingCancelledEvent closingCancelledEvent)
        {
            IsCancelled = true;

            return this;
        }
    }
}
