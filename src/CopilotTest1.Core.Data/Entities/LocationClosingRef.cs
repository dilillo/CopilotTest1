using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.Core.Data.Entities
{
    public class LocationClosingRef
    {
        [Key]
        public Guid Id { get; set; }

        public Guid LocationId { get; set; }

        public LocationRef Location { get; set; } = null!;

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }
    }
}

