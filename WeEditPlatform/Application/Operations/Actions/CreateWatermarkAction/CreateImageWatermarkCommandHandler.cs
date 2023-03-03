using MediatR;
using Infrastructure.Services;

namespace Application.Operations.Actions.CreateWatermarkAction
{
    public class CreateImageWatermarkCommandHandler : IRequestHandler<CreateImageWatermarkCommand>
    {
        private readonly IWatermark _watermark;

        public CreateImageWatermarkCommandHandler(IWatermark watermark)
        {
            _watermark = watermark;
        }

        public async Task<Unit> Handle(CreateImageWatermarkCommand request, CancellationToken cancellationToken)
        {
            _watermark.CreateImageWatermark(request.Session, request.InputFile, request.ImageWatermark, request.ImageMarkOptions);
            return Unit.Value;
        }

    }
}
