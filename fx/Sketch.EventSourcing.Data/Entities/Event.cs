using System.ComponentModel.DataAnnotations;

namespace Sketch.EventSourcing.Data.Entities
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }

        public Guid AggregateId { get; set; }

        public int Version { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventType { get; set; } = null!;

        [Required]
        public string Payload { get; set; } = null!;

        public bool IsPublished { get; set; } = false;
    }
}
