using CopilotTest1.Core.Domain.Locations;
using CopilotTest1.Core.Domain.Providers;
using CopilotTest1.Core.Infrastructure;
using CopilotTest1.Core.Locations;
using Microsoft.AspNetCore.Mvc;

namespace CopilotTest1.Core.WebApi.Locations
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class LocationsController : Controller
    {
        private readonly IGrainFactory _grainFactory;

        public LocationsController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet("[controller]/{id:guid})")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProviderState))]
        public async Task<IActionResult> Get(Guid id)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);
            var state = await grain.GetState();

            return Ok(state);
        }

        [HttpPost("[controller]/open")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Open([FromBody] LocationState value)
        {
            var id = Guid.NewGuid();
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.Open(value.BusinessId, value.Profile, value.Hours);

            return CreatedAtAction(nameof(Get), new { id });
        }

        [HttpDelete("[controller]/{id:guid}")]
        public async Task<IActionResult> Close(Guid id)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.PermanentlyClose();

            return NoContent();
        }

        [HttpPut("[controller]/{id:guid}/profile")]
        public async Task<IActionResult> ModifyProfile(Guid id, [FromBody] LocationProfile value)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.ModifyProfile(value);

            return NoContent();
        }

        [HttpPut("[controller]/{id:guid}/hours")]
        public async Task<IActionResult> ModifyHours(Guid id, [FromBody] List<DayOfOperation> value)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.ModifyHours(value);

            return NoContent();
        }

        [HttpPost("[controller]/{id:guid}/services")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddService([FromQuery] Guid id, [FromBody] LocationService value)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            value.Id = Guid.NewGuid();

            await grain.AddService(value);

            return CreatedAtAction(nameof(Get), new { id, serviceId = value.Id });
        }

        [HttpPut("[controller]/{id:guid}/services")]
        public async Task<IActionResult> ModifyService(Guid id, [FromBody] LocationService value)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.ModifyService(value);

            return NoContent();
        }

        [HttpDelete("[controller]/{id:guid}/services/{locationServiceId:guid}")]
        public async Task<IActionResult> RemoveService(Guid id, Guid locationServiceId)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.RemoveService(locationServiceId);

            return NoContent();
        }

        [HttpPost("[controller]/{id:guid}/serviceproviders")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddServiceProvider([FromQuery] Guid id, [FromBody] LocationServiceProvider value)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.AddServiceProvider(value);

            return CreatedAtAction(nameof(Get), new { id });
        }

        [HttpDelete("[controller]/{id:guid}/serviceproviders")]
        public async Task<IActionResult> RemoveServiceProvider(Guid id, [FromBody] LocationServiceProvider value)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.RemoveServiceProvider(value);

            return NoContent();
        }

        [HttpPost("[controller]/{id:guid}/customers/{customerId:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddCustomer([FromQuery] Guid id, [FromQuery] Guid customerId)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.AddCustomer(customerId);

            return CreatedAtAction(nameof(Get), new { id });
        }

        [HttpDelete("[controller]/{id:guid}/customers/{customerId:guid}")]
        public async Task<IActionResult> RemoveCustomer(Guid id, [FromQuery] Guid customerId)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.RemoveCustomer(customerId);

            return NoContent();
        }

        [HttpPost("[controller]/{id:guid}/closings")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddClosing([FromQuery] Guid id, [FromBody] LocationClosing value)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            value.Id = Guid.NewGuid();

            await grain.AddClosing(value);

            return CreatedAtAction(nameof(Get), new { id, locationClosingId = value.Id });
        }

        [HttpDelete("[controller]/{id:guid}/closings/{locationClosingId:guid}")]
        public async Task<IActionResult> RemoveClosing(Guid id, Guid locationClosingId)
        {
            var grain = _grainFactory.GetGrain<ILocationAggregate>(id);

            await grain.RemoveClosing(locationClosingId);

            return NoContent();
        }
    }
}
