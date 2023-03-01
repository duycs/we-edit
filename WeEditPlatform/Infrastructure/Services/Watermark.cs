using System;
using System.Drawing;
using Infrastructure.Extensions;
using LazZiya.ImageResize;
using Microsoft.AspNetCore.Components.Forms;
using Org.BouncyCastle.Utilities.Encoders;

namespace Infrastructure.Services
{
	public class Watermark : IWatermark
	{
		public Watermark()
		{
		}

        public string CreateCombineWatermark(string inputFile, string textWatermark, TextMarkOptions textMarkOptions, string imageWatermark, ImageMarkOptions imageMarkOptions)
        {
            string outputFileName = GetOutputFileName(inputFile, "combineMarked");
            string scaleWatermarkFileName = ScaleImageWaterMark(imageWatermark, imageMarkOptions);
            var twmOps = SetTextWaterMarkOption(textMarkOptions);
            var iwmOps = SetImageWaterMarkOptions(imageMarkOptions);

            using (var img = Image.FromFile(inputFile))
            {
                var iwm = Image.FromFile(scaleWatermarkFileName);

                img.ScaleByWidth(imageMarkOptions.ImageScaleByWith == 0 ? img.Width : imageMarkOptions.ImageScaleByWith)
                    .AddImageWatermark(iwm, iwmOps)
                    .AddTextWatermark(textWatermark, twmOps)
                    .SaveAs(outputFileName);

                iwm.Dispose();
            }

            return outputFileName;
        }

        public string CreateImageWatermark(string inputFile, string imageWatermark, ImageMarkOptions imageMarkOptions)
        {
            string outputFileName = GetOutputFileName(inputFile, "imageMarked");
            string scaleWatermarkFileName = ScaleImageWaterMark(imageWatermark, imageMarkOptions);
            var iwmOps = SetImageWaterMarkOptions(imageMarkOptions);

            using (var img = Image.FromFile(inputFile))
            {
                var iwm = Image.FromFile(scaleWatermarkFileName);

                img.ScaleByWidth(imageMarkOptions.ImageScaleByWith == 0 ? img.Width : imageMarkOptions.ImageScaleByWith)
                    .AddImageWatermark(iwm, iwmOps)
                    .SaveAs(outputFileName);

                iwm.Dispose();
            }

            return outputFileName;
        }


        public string CreateTextWatermark(string inputFile, string textWatermark, TextMarkOptions textMarkOptions)
        {
            string outputFileName = GetOutputFileName(inputFile, "textMarked");
            var twmOps = SetTextWaterMarkOption(textMarkOptions);

            using (var img = Image.FromFile(inputFile))
            {
                img.ScaleByWidth(textMarkOptions.ImageScaleByWith == 0 ? img.Width : textMarkOptions.ImageScaleByWith)
                    .AddTextWatermark(textWatermark, twmOps)
                    .SaveAs(outputFileName);
            }

            return outputFileName;
        }

        private TextWatermarkOptions SetTextWaterMarkOption(TextMarkOptions textMarkOptions)
        {
            var twmOps = new TextWatermarkOptions
            {
                Location = (TargetSpot)textMarkOptions.Location,

                FontSize = textMarkOptions.FontSize == 0 ? 24 : textMarkOptions.FontSize,

                FontName = textMarkOptions.FontName == "" ? "Arial" : textMarkOptions.FontName,

                FontStyle = (FontStyle)textMarkOptions.FontStyle,

                TextColor = Color.FromArgb(textMarkOptions.Opacity, GetColor(string.IsNullOrEmpty(textMarkOptions.TextColor) ? "White" : textMarkOptions.TextColor)),

                // Use alpha channel to specify color opacity
                // e.g. use 0 opacity to disable drawing the outline
                OutlineColor = Color.FromArgb(textMarkOptions.Opacity, GetColor(string.IsNullOrEmpty(textMarkOptions.OutlineColor) ? "White" : textMarkOptions.OutlineColor))

            };

            return twmOps;
        }

        private ImageWatermarkOptions SetImageWaterMarkOptions(ImageMarkOptions imageMarkOptions)
        {
            var iwmOps = new ImageWatermarkOptions
            {
                Location = (TargetSpot)imageMarkOptions.Location,

                // 0 full transparent, 100 full color
                Opacity = imageMarkOptions.Opacity,
            };

            return iwmOps;
        }

        private string ScaleImageWaterMark(string imageWatermark, ImageMarkOptions imageMarkOptions)
        {
            string scaleWatermarkFileName = @$"{Path.GetFileName(imageWatermark)}-scaled.png";

            // resize watermark image
            using (var img = Image.FromFile(imageWatermark))
            {
                img.ScaleByWidth(imageMarkOptions.WatermarkScaleByWith == 0 ? 100 : imageMarkOptions.WatermarkScaleByWith)
                    .SaveAs(scaleWatermarkFileName);
            }

            return scaleWatermarkFileName;
        }

        private string GetOutputFileName(string inputFile, string endFixName)
        {
            string watermarkFolder = FileExtension.GetImageWatermarkFolder();
            string outputFileName = @$"{watermarkFolder}\\{Path.GetFileName(inputFile).Split('.')[0]}-{endFixName}.jpg";
            return outputFileName;
        }

        private Color GetColor(string color)
        {
            switch (color)
            {
                case "Black":
                    return Color.Black;

                case "Blue":
                    return Color.Blue;

                case "Brown":
                    return Color.Brown;

                case "Cyan":
                    return Color.Cyan;

                case "Gray":
                    return Color.Gray;

                case "Green":
                    return Color.Green;

                case "Lime":
                    return Color.Lime;

                case "Magenta":
                    return Color.Magenta;

                case "Navy":
                    return Color.Navy;

                case "Orange":
                    return Color.Orange;

                case "Pink":
                    return Color.Pink;

                case "White":
                    return Color.White;

                case "Red":
                    return Color.Red;

                case "Silver":
                    return Color.Silver;

                case "Yellow":
                    return Color.Yellow;

                default:
                    return Color.White;
            }
        }
    }
}


public class ImageMarkOptions
{
    /// <summary>
    /// 8: bottom
    /// </summary>
    public int Location { get; set; }
    public int ImageScaleByWith { get; set; }

    /// <summary>
    /// 0 full transparent, 100 full color
    /// </summary>
    public int Opacity { get; set; }

    public int WatermarkScaleByWith { get; set; }
}

public class TextMarkOptions
{
    public int Location { get; set; }
    public int ImageScaleByWith { get; set; }
    public int FontSize { get; set; }
    public int FontStyle { get; set; }
    public string? FontName { get; set; } = "";
    public string? TextColor { get; set; } = "";
    public string? OutlineColor { get; set; } = "";
    public int Opacity { get; set; } = 100;

}