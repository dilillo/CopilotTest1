using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.People.WebApi.Customers
{
    public class CustomerProfileDto
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
