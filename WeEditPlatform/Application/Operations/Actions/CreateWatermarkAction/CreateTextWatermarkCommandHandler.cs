using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.IO;
using Infrastructure.Services;

namespace Application.Operations.Actions.CreateWatermarkAction
{
	public class CreateTextWatermarkCommandHandler : IRequestHandler<CreateTextWatermarkCommand>
    {
        private readonly IWatermark _watermark;

        public CreateTextWatermarkCommandHandler(IWatermark watermark)
        {
            _watermark = watermark;
        }

        public async Task<Unit> Handle(CreateTextWatermarkCommand request, CancellationToken cancellationToken)
        {
            _watermark.CreateTextWatermark(request.InputFile, request.TextWatermark, request.TextMarkOptions);
            return Unit.Value;
        }
    }
}

