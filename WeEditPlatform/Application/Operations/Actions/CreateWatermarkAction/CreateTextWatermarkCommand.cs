using MediatR;
using System;

namespace Application.Operations.Actions.CreateWatermarkAction
{
	public class CreateTextWatermarkCommand : IRequest
    {
        public string Session { get; set; }
        public string InputFile { get; set; }
        public string TextWatermark { get; set; }
        public TextMarkOptions TextMarkOptions { get; set; }
	}
}

