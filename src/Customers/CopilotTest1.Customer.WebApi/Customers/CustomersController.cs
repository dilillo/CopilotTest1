using AutoMapper;
using CopilotTest1.People.Domain.Customers;
using CopilotTest1.Shared.Domain.Customers;
using Microsoft.AspNetCore.Mvc;

namespace CopilotTest1.People.WebApi.Customers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class CustomersController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;
        private readonly IMapper _mapper;

        public CustomersController(IGrainFactory grainFactory, IMapper mapper)
        {
            _grainFactory = grainFactory;
            _mapper = mapper;
        }

        [HttpGet("[controller]/{id:guid})")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDto))]
        public async Task<IActionResult> Get(Guid id)
        {
            var grain = _grainFactory.GetGrain<ICustomerAggregate>(id);
            var state = await grain.GetState();
            var result = _mapper.Map<CustomerDto>(state);

            return Ok(result);
        }

        [HttpPost("[controller]/register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromBody] CustomerProfileDto value)
        {
            var id = Guid.NewGuid();
            var grain = _grainFactory.GetGrain<ICustomerAggregate>(id);
            var profile = _mapper.Map<CustomerProfile>(value);

            await grain.Register(profile);

            return CreatedAtAction(nameof(Get), new { id });
        }

        [HttpPut("[controller]/{id:guid}/profile")]
        public async Task<IActionResult> ModifyProfile(Guid id, [FromBody] CustomerProfileDto value)
        {
            var grain = _grainFactory.GetGrain<ICustomerAggregate>(id);
            var profile = _mapper.Map<CustomerProfile>(value);

            await grain.ModifyProfile(profile);

            return NoContent();
        }

        [HttpPost("[controller]/{id:guid}/providers/{providerId:guid}")]
        public async Task<IActionResult> AddProvider(Guid id, Guid providerId)
        {
            var grain = _grainFactory.GetGrain<ICustomerAggregate>(id);

            await grain.AddProvider(providerId);

            return NoContent();
        }

        [HttpDelete("[controller]/{id:guid}/providers/{providerId:guid}")]
        public async Task<IActionResult> RemoveService(Guid id, Guid providerId)
        {
            var grain = _grainFactory.GetGrain<ICustomerAggregate>(id);

            await grain.RemoveProvider(providerId);

            return NoContent();
        }
    }
}
