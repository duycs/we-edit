using System;
using System.Drawing;
using LazZiya.ImageResize;

namespace Infrastructure.Services
{
	public class Watermark : IWatermark
	{
		public Watermark()
		{
		}

        public string CreateCombineWatermark(string inputFile, string textWatermark, TextMarkOptions textMarkOptions, string imageWatermark, ImageMarkOptions imageMarkOptions)
        {
            var textWatermarked = CreateTextWatermark(inputFile, textWatermark, textMarkOptions);
            var imageWatermarked = CreateImageWatermark(textWatermarked, imageWatermark, imageMarkOptions);

            return imageWatermarked;
        }

        public string CreateImageWatermark(string inputFile, string imageWatermark, ImageMarkOptions imageMarkOptions)
        {
            var folderName = Path.Combine("Resources", "Images\\Watermarks");
            string outputFileName = @$"{folderName}\\{Path.GetFileName(inputFile).Split('.')[0]}-imageMarked.jpg";
            string scaleWatermarkFileName = @$"{Path.GetFileName(imageWatermark)}-scaled.png";

            // resize watermark image
            using (var img = Image.FromFile(imageWatermark))
            {
                img.ScaleByWidth(imageMarkOptions.WatermarkScaleByWith == 0 ? 100 : imageMarkOptions.WatermarkScaleByWith)
                    .SaveAs(scaleWatermarkFileName);
            }

            var iwmOps = new ImageWatermarkOptions
            {
                Location = (TargetSpot)imageMarkOptions.Location,

                Opacity = imageMarkOptions.Opacity,
            };

            using (var img = Image.FromFile(inputFile))
            {
                var iwm = Image.FromFile(scaleWatermarkFileName);

                img.AddImageWatermark(iwm, iwmOps)
                    .SaveAs(outputFileName);

                iwm.Dispose();
            }

            return outputFileName;
        }


        public string CreateTextWatermark(string inputFile, string textWatermark, TextMarkOptions textMarkOptions)
        {
            var folderName = Path.Combine("Resources", "Images\\Watermarks");
            string outputFileName = @$"{folderName}\\{Path.GetFileName(inputFile).Split('.')[0]}-textMarked.jpg";

            var twmOps = new TextWatermarkOptions
            {
                Location = (TargetSpot)textMarkOptions.Location,

                FontSize = textMarkOptions.FontSize == 0 ? 24 : textMarkOptions.FontSize,

                FontName = textMarkOptions.FontName == "" ? "Arial" : textMarkOptions.FontName,

                // Text with red color and half opacity
                TextColor = Color.FromArgb(200, Color.Red),

                // Use alpha channel to specify color opacity
                // e.g. use 0 opacity to disable drawing the outline
                OutlineColor = Color.FromArgb(0, Color.White),
            };

            using (var img = Image.FromFile(inputFile))
            {
                img //.Crop(500, 500)
                    .AddTextWatermark(textWatermark, twmOps)
                    .SaveAs(outputFileName);
            }

            return outputFileName;
        }
    }
}


public class ImageMarkOptions
{
    /// <summary>
    /// 8: bottom
    /// </summary>
    public int Location { get; set; }

    /// <summary>
    /// 0 full transparent, 100 full color
    /// </summary>
    public int Opacity { get; set; }

    public int WatermarkScaleByWith { get; set; }
}

public class TextMarkOptions
{
    public int Location { get; set; }
    public int FontSize { get; set; }
    public string FontName { get; set; }
}