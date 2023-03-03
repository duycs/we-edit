using System.Drawing;
using LazZiya.ImageResize;

namespace Infrastructure.Services
{

    public interface IWatermark
    {
        public string CreateImageWatermark(string session, string inputFile, string imageWatermark, ImageMarkOptions imageMarkOptions);
        public string CreateTextWatermark(string session, string inputFile, string textWatermark, TextMarkOptions textMarkOptions);
        public string CreateCombineWatermark(string session, string inputFile, string textWatermark, TextMarkOptions textMarkOptions,
            string imageWatermark, ImageMarkOptions imageMarkOptions);
    }
}