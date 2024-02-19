using System.ComponentModel.DataAnnotations;

namespace Sketch.EventSourcing.Data.Entities
{
    public class Snapshot
    {
        [Key]
        [Required]
        [MaxLength(100)]
        public Guid AggregateId { get; set; }

        public int Version { get; set; }

        [Required]
        public string Payload { get; set; } = null!;
    }
}
