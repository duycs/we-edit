using System;
using Application.Operations.Actions.CreateWatermarkAction;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Houzz.Api.Controllers.WebControllers
{
    [ApiController]
    [Route("[controller]")]
    public class WatermarksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WatermarksController(IMediator mediator)
		{
            _mediator = mediator;
		}

        [HttpPost("image")]
        public async Task<IActionResult> CreateImageWatermark([FromBody] CreateImageWatermarkCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("text")]
        public async Task<IActionResult> CreateTextWatermark([FromBody] CreateTextWatermarkCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("combine")]
        public async Task<IActionResult> CreateCombineWatermark([FromBody] CreateCombineWatermarkCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

    }
}

