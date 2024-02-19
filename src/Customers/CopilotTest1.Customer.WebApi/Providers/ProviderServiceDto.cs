using System.ComponentModel.DataAnnotations;

namespace CopilotTest1.People.WebApi.Providers
{
    public class ProviderServiceDto
    {
        [Required]
        public Guid LocationServiceId { get; set; }
    }
}
