using CopilotTest1.Core.Domain.Providers;
using CopilotTest1.Core.Providers;
using Microsoft.AspNetCore.Mvc;

namespace CopilotTest1.Core.WebApi.Providers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ProvidersController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;

        public ProvidersController(IGrainFactory grainFactory)
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

        [HttpPut("[controller]/{id:guid}/hours")]
        public async Task<IActionResult> ModifyHours(Guid id, [FromBody] List<ProviderLocationDayOfOperation> value)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);

            await grain.ModifyHours(value);

            return NoContent();
        }

        [HttpPost("[controller]/{id:guid}/appointments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> BookAppointment([FromQuery]Guid id, [FromBody] ProviderServiceAppointment value)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);

            value.Id = Guid.NewGuid();

            await grain.BookAppointment(value);

            return CreatedAtAction(nameof(Get), new { id, appointmentId = value.Id });
        }

        [HttpPut("[controller]/{id:guid}/appointments/{appointmentId:guid}")]
        public async Task<IActionResult> CompleteAppointment(Guid id, Guid appointmentId)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);

            await grain.CompleteAppointment(appointmentId);

            return NoContent();
        }

        [HttpDelete("[controller]/{id:guid}/appointments/{appointmentId:guid}")]
        public async Task<IActionResult> CancelAppointment(Guid id, Guid appointmentId)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);

            await grain.CancelAppointment(appointmentId);

            return NoContent();
        }

        [HttpPost("[controller]/{id:guid}/timeoffs")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> TakeTimeOff([FromQuery] Guid id, [FromBody] ProviderTimeOff value)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);

            value.Id = Guid.NewGuid();

            await grain.TakeTimeOff(value);

            return CreatedAtAction(nameof(Get), new { id, timeOffId = value.Id });
        }

        [HttpDelete("[controller]/{id:guid}/timeoffs/{timeOffId:guid}")]
        public async Task<IActionResult> CancelTimeOff(Guid id, Guid timeOffId)
        {
            var grain = _grainFactory.GetGrain<IProviderAggregate>(id);

            await grain.CancelTimeOff(timeOffId);

            return NoContent();
        }
    }
}
