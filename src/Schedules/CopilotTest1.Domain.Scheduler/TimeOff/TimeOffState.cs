using CopilotTest1.Shared.Domain.TimeOff;

namespace CopilotTest1.Scheduler.Domain.TimeOff
{
    public class TimeOffState
    {
        public Guid ProviderId { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        public string Reason { get; set; } = string.Empty;

        public bool IsCancelled { get; set; }

        public TimeOffState Apply(TimeOffPostedEvent timeOffPostedEvent)
        {
            ProviderId = timeOffPostedEvent.Request.ProviderId;
            Start = timeOffPostedEvent.Request.Start;
            End = timeOffPostedEvent.Request.End;
            Reason = timeOffPostedEvent.Request.Reason;

            return this;
        }

        public TimeOffState Apply(TimeOffCancelledEvent timeOffCancelledEvent)
        {
            IsCancelled = true;

            return this;
        }
    }
}
