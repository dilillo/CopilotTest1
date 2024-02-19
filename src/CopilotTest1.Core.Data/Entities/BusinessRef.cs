using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.Core.Data.Entities
{
    public class BusinessRef
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsPermanentlyClosed { get; set; }

        public ICollection<LocationRef> Locations { get; } = new List<LocationRef>();
    }
}
