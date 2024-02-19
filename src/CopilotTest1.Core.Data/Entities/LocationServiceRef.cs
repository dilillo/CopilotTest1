using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.Core.Data.Entities
{
    public class LocationServiceRef
    {
        [Key]
        public Guid Id { get; set; }

        public Guid LocationId { get; set; }

        public LocationRef Location { get; set; } = null!;

        public int DurationInMinutes { get; set; }

        public ICollection<LocationServiceProviderRef> LocationServiceProviders { get; } = new List<LocationServiceProviderRef>();
    }
}
