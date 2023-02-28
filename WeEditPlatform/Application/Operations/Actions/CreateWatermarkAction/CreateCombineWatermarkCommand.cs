using MediatR;
using System;

namespace Application.Operations.Actions.CreateWatermarkAction
{
	public class CreateCombineWatermarkCommand : IRequest
    {
		public string InputFile { get; set; }

		public string ImageWatermark { get; set; }
		public ImageMarkOptions ImageMarkOptions { get; set; }

        public string TextWatermark { get; set; }
        public TextMarkOptions TextMarkOptions { get; set; }
    }
}

