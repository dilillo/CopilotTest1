using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.People.Data.Entities
{
    public class ProviderRef
    {
        [Key]
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;
    }
}
