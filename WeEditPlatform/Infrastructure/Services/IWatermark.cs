using System.Drawing;
using LazZiya.ImageResize;

namespace Infrastructure.Services
{

    public interface IWatermark
    {
        public string CreateImageWatermark(string inputFile, string imageWatermark, ImageMarkOptions imageMarkOptions);
        public string CreateTextWatermark(string inputFile, string textWatermark, TextMarkOptions textMarkOptions);
        public string CreateCombineWatermark(string inputFile, string textWatermark, TextMarkOptions textMarkOptions,
            string imageWatermark, ImageMarkOptions imageMarkOptions);
    }
}