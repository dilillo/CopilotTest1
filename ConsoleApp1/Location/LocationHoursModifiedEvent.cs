using ConsoleApp1.Infrastructure;

namespace ConsoleApp1.Location
{
    internal class LocationHoursModifiedEvent
    {
        public List<DayOfOperation> Hours { get; set; }
    }
}