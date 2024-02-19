using AutoMapper;
using CopilotTest1.Core.Businesses;
using CopilotTest1.Core.Domain.Businesses;
using Microsoft.AspNetCore.Mvc;

namespace CopilotTest1.Core.WebApi.Businesses
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class BusinessesController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;

        public BusinessesController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet("[controller]/{id:guid})")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BusinessState))]
        public async Task<IActionResult> Get(Guid id)
        {
            var grain = _grainFactory.GetGrain<IBusinessAggregate>(id);
            var state = await grain.GetState();

            return Ok(state);
        }

        [HttpPost("[controller]/open")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Open([FromBody] BusinessState value)
        {
            var id = Guid.NewGuid();
            var grain = _grainFactory.GetGrain<IBusinessAggregate>(id);

            await grain.Open(value.Operators.FirstOrDefault(), value.Profile);

            return CreatedAtAction(nameof(Get), new { id });
        }

        [HttpPut("[controller]/{id:guid}/profile")]
        public async Task<IActionResult> ModifyProfile(Guid id, [FromBody] BusinessProfile value)
        {
            var grain = _grainFactory.GetGrain<IBusinessAggregate>(id);

            await grain.ModifyProfile(value);

            return NoContent();
        }

        [HttpPost("[controller]/{id:guid}/operators/{operatorId:guid}")]
        public async Task<IActionResult> AddOperator(Guid id, Guid operatorId)
        {
            var grain = _grainFactory.GetGrain<IBusinessAggregate>(id);

            await grain.AddOperator(operatorId);

            return NoContent();
        }

        [HttpDelete("[controller]/{id:guid}/providers/{operatorId:guid}")]
        public async Task<IActionResult> RemoveOperator(Guid id, Guid operatorId)
        {
            var grain = _grainFactory.GetGrain<IBusinessAggregate>(id);

            await grain.RemoveOperator(operatorId);

            return NoContent();
        }

        [HttpDelete("[controller]/{id:guid}")]
        public async Task<IActionResult> Close(Guid id)
        {
            var grain = _grainFactory.GetGrain<IBusinessAggregate>(id);

            await grain.PermanentlyClose();

            return NoContent();
        }
    }
}
