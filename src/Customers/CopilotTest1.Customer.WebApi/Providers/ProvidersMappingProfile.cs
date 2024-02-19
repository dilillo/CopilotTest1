using AutoMapper;
using CopilotTest1.People.Domain.Providers;
using CopilotTest1.Shared.Domain.Providers;

namespace CopilotTest1.People.WebApi.Providers
{
    public class ProvidersMappingProfile : Profile
    {
        public ProvidersMappingProfile()
        {
            CreateMap<ProviderState, ProviderDto>();
            CreateMap<ProviderProfile, ProviderProfileDto>();

            CreateMap<ProviderProfileDto, ProviderProfile>();
        }
    }
}
