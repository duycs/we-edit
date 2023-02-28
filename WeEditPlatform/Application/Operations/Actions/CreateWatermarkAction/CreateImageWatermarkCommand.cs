using MediatR;
using System;

namespace Application.Operations.Actions.CreateWatermarkAction
{
	public class CreateImageWatermarkCommand : IRequest
    {
		public string InputFile { get; set; }
		public string ImageWatermark { get; set; }
		public ImageMarkOptions ImageMarkOptions { get; set; }
    }
}

