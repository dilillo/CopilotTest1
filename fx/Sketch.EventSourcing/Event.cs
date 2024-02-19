namespace Sketch.EventSourcing
{
    public abstract class Event
    {
        public Guid Id { get; set; }

        public Guid AggregateId { get; set; }

        public DateTimeOffset OccurredOn { get; set; }
    }
}
