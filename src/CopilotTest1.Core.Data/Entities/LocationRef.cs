using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.Core.Data.Entities
{
    public class LocationRef
    {
        [Key]
        public Guid Id { get; set; }

        public Guid BusinessId { get; set; }

        public BusinessRef Business { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public bool IsPermanentlyClosed { get; set; }

        public ICollection<LocationServiceRef> LocationServices { get; } = new List<LocationServiceRef>();

        public ICollection<LocationClosingRef> LocationClosings { get; } = new List<LocationClosingRef>();

        public ICollection<LocationHourRef> LocationHours { get; } = new List<LocationHourRef>();
    }
}
