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
        private readonly IWatermark _watermark;

        public WatermarksController(IMediator mediator, IWatermark watermark)
		{
            _mediator = mediator;
            _watermark = watermark;
		}

        [HttpPost("image")]
        public async Task<IActionResult> CreateImageWatermark([FromBody] CreateImageWatermarkCommand command)
        {
            var result = await _mediator.Send(command);
            //var result = _watermark.CreateImageWatermark(command.InputFile, command.ImageWatermark, command.ImageMarkOptions);
            return Ok(result);
        }

        [HttpPost("text")]
        public async Task<IActionResult> CreateTextWatermark([FromBody] CreateTextWatermarkCommand command)
        {
            var result = await _mediator.Send(command);
            //var result = _watermark.CreateTextWatermark(command.InputFile, command.TextWatermark, command.TextMarkOptions);
            return Ok(result);
        }

        [HttpPost("combine")]
        public async Task<IActionResult> CreateCombineWatermark([FromBody] CreateCombineWatermarkCommand command)
        {
            var result = await _mediator.Send(command);
            //var result = _watermark.CreateCombineWatermark(command.InputFile, command.TextWatermark, command.TextMarkOptions,
                //command.ImageWatermark, command.ImageMarkOptions);
            return Ok(result);
        }

    }
}

