using AutoMapper;
using CopilotTest1.People.Domain.Customers;
using CopilotTest1.Shared.Domain.Customers;

namespace CopilotTest1.People.WebApi.Customers
{
    public class CustomersMappingProfile : Profile
    {
        public CustomersMappingProfile()
        {
            CreateMap<CustomerState, CustomerDto>();
            CreateMap<CustomerProfile, CustomerProfileDto>();

            CreateMap<CustomerProfileDto, CustomerProfile>();
        }
    }
}
