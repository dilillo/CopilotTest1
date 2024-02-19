using AutoMapper;
using CopilotTest1.People.Domain.Providers;
using CopilotTest1.People.WebApi.Customers;
using CopilotTest1.Shared.Domain.Providers;
using Microsoft.AspNetCore.Mvc;

namespace CopilotTest1.People.WebApi.Providers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ProvidersController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;
        private readonly IMapper _mapper;

        public ProvidersController(IGrainFactory grainFactory, IMapper mapper)
        {
            _grainFactory = grainFactory;
            _mapper = mapper;
        }

        [HttpGet("[controller]/{id:guid})")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDto))]
        public async Task<IActionResult> Get(Guid id)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);
            var state = await grain.GetState();
            var result = _mapper.Map<ProviderDto>(state);

            return Ok(result);
        }

        [HttpPost("[controller]/register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromBody] ProviderProfileDto value)
        {
            var id = Guid.NewGuid();
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);
            var profile = _mapper.Map<ProviderProfile>(value);

            await grain.CancelService(profile);

            return CreatedAtAction(nameof(Get), new { id });
        }

        [HttpPut("[controller]/{id:guid}/profile")]
        public async Task<IActionResult> ModifyProfile(Guid id, [FromBody] ProviderProfileDto value)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);
            var profile = _mapper.Map<ProviderProfile>(value);

            await grain.ModifyProfile(profile);

            return NoContent();
        }

        [HttpPost("[controller]/{id:guid}/service")]
        public async Task<IActionResult> ProviderService(Guid id, [FromBody] ProviderServiceDto value)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);

            await grain.RequestAppointment(value.LocationServiceId);

            return NoContent();
        }

        [HttpDelete("[controller]/{id:guid}/services/{locationServiceId:guid}")]
        public async Task<IActionResult> RemoveService(Guid id, Guid locationServiceId)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);

            await grain.BookService(locationServiceId);

            return NoContent();
        }
    }
}
