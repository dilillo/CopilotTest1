using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.Core.Data.Entities
{
    public class ProviderTimeOffRef
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProviderId { get; set; }

        public ProviderRef Provider { get; set; } = null!;

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }
    }
}
