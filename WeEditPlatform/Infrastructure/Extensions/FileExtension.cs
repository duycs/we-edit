using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class FileExtension
    {
        public static string GetImageWatermarkFolder(string session)
        {
            return Path.Combine("Resources", "Images", "Watermarks", session);
        }

        public static string GetImageWatermarkFolderTemp(string session)
        {
            return Path.Combine(GetImageWatermarkFolder(session), "Temp");
        }

        public static bool IsAPhotoFile(this string fileName)
        {
            return fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".webp", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        public static string RemoveSuffix(this string filePath)
        {
            return filePath.Split("?")[0];
        }

        public static List<string> RemoveSuffix(this List<string> filePaths)
        {
            return filePaths.Select(f => f.RemoveSuffix()).ToList();
        }
    }
}
