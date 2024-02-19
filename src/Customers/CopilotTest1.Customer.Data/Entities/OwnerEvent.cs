using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.Customer.Data.Entities
{
    public class PlaceEvent
    {
        [Key]
        public Guid Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventType { get; set; } = null!;

        [Required]
        public string Payload { get; set; } = null!;

        public bool IsProcessed { get; set; } = false;

        public int ProcessAttempts { get; set; } = 0;
    }
}
