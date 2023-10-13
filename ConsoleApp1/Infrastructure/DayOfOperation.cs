namespace ConsoleApp1.Infrastructure
{
    public class DayOfOperation
    {
        public DayOfWeek Day { get; set; }

        public List<PeriodOfOperation> Hours { get; set; } = new List<PeriodOfOperation>();
    }

    public class PeriodOfOperation
    {
        public TimeOnly StartTime { get; set; }

        public TimeOnly StopTime { get; set; }
    }
}
