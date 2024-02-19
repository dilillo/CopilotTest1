namespace CopilotTest1.Core.Infrastructure
{
    [GenerateSerializer]
    public class DayOfOperation
    {
        [Id(0)]
        public DayOfWeek Day { get; set; }

        [Id(1)]
        public List<PeriodOfOperation> Hours { get; set; } = new List<PeriodOfOperation>();
    }

    [GenerateSerializer]
    public class PeriodOfOperation
    {
        [Id(0)]
        public byte StartHour { get; set; }

        [Id(1)]
        public byte StartMinute { get; set; }

        public TimeSpan StartTime => new TimeSpan(StartHour, StartMinute, 0);

        [Id(3)]
        public byte EndHour { get; set; }

        [Id(4)]
        public byte EndMinute { get; set; }

        public TimeSpan EndTime => new TimeSpan(EndHour, EndMinute, 0);
    }
}
