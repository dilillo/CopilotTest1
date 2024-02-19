using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.Customer.Data.Entities
{
    public class CustomerRef
    {
        [Key]
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;
    }
}
