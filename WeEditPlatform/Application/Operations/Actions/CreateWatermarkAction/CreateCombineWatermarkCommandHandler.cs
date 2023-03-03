using MediatR;
using Infrastructure.Services;

namespace Application.Operations.Actions.CreateWatermarkAction
{
    public class CreateCombineWatermarkCommandHandler : IRequestHandler<CreateCombineWatermarkCommand>
    {
        private readonly IWatermark _watermark;

        public CreateCombineWatermarkCommandHandler(IWatermark watermark)
        {
            _watermark = watermark;
        }

        public async Task<Unit> Handle(CreateCombineWatermarkCommand request, CancellationToken cancellationToken)
        {
            _watermark.CreateCombineWatermark(request.Session, request.InputFile, request.TextWatermark, request.TextMarkOptions,
                request.ImageWatermark, request.ImageMarkOptions);

            return Unit.Value;
        }

    }
}
