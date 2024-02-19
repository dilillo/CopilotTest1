using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.Customer.Data.Entities
{
    public class BusinessRef
    {
        [Key]
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;
    }
}
