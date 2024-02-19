namespace CopilotTest1.Core.Data.Entities
{
    public class ProviderHourRef
    {
        public Guid ProviderId { get; set; }

        public ProviderRef Provider { get; set; } = null!;

        public Guid LocationId { get; set; }

        public LocationRef Location { get; set; } = null!;

        public DayOfWeek Day { get; set; }

        public byte StartHour { get; set; }

        public byte StartMinute { get; set; }

        public TimeSpan StartTime => new TimeSpan(StartHour, StartMinute, 0);

        public byte EndHour { get; set; }

        public byte EndMinute { get; set; }

        public TimeSpan EndTime => new TimeSpan(EndHour, EndMinute, 0);
    }
}
