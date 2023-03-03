using Application.Models;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.IO.Compression;

namespace Houzz.Api.Controllers.WebControllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _host;

        public FilesController(IWebHostEnvironment host)
        {
            _host = host;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                StringValues session = "";
                formCollection.TryGetValue("Session", out session);
                var file = formCollection.Files.First();
                var folderName = FileExtension.GetImageWatermarkFolderTemp(session.ToString());
                var pathToSave = Path.Combine(_host.WebRootPath, folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim();
                    var fullPath = Path.Combine(pathToSave, fileName.ToString());
                    var dbPath = Path.Combine(folderName, fileName.ToString());
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("download")]
        public async Task<IActionResult> Download([FromQuery] string fileUrls, [FromQuery] bool isZip = false)
        {
            if (string.IsNullOrEmpty(fileUrls))
            {
                return NotFound();
            }

            var fileUrlArr = fileUrls.Split(",").ToList();

            // download zip images
            if (isZip || fileUrlArr.Count > 1)
            {
                return ZipFiles(fileUrlArr);
            }


            // download single image
            var filePath = Path.Combine(_host.WebRootPath, fileUrlArr[0]).RemoveSuffix();

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, FileExtension.GetContentType(filePath), filePath);

        }

        [HttpGet]
        [Route("remove")]
        public async Task<IActionResult> Remove([FromQuery] string fileUrls)
        {
            if (string.IsNullOrEmpty(fileUrls))
            {
                return NotFound();
            }

            var fileUrlArr = fileUrls.Split(",").ToList();

            foreach (var fileUrl in fileUrlArr)
            {
                var filePath = Path.Combine(_host.WebRootPath, fileUrl).RemoveSuffix();

                if (!System.IO.File.Exists(filePath))
                    continue;

                System.IO.File.Delete(filePath);
            }

            return NoContent();
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("photos")]
        public IActionResult GetPhotos([FromQuery] string Session)
        {
            try
            {
                if (string.IsNullOrEmpty(Session))
                {
                    return NotFound();
                }

                var folderName = FileExtension.GetImageWatermarkFolder(Session);
                var pathToRead = Path.Combine(_host.WebRootPath, folderName);

                if (!Directory.Exists(pathToRead))
                {
                    return NotFound();
                }

                var photos = Directory.EnumerateFiles(pathToRead)
                    .Where(fullPath => fullPath.IsAPhotoFile())
                    .Select(fullPath => new PhotoDto(@$"{Path.Combine(folderName, Path.GetFileName(fullPath))}?{DateTime.UtcNow.Ticks}")).ToArray();

                return Ok(photos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


        private FileStreamResult ZipFiles(List<string> fileUrls)
        {
            var filePaths = new List<string>();

            foreach (var fileUrl in fileUrls)
            {
                var filePath = Path.Combine(_host.WebRootPath, fileUrl).RemoveSuffix();
                if (System.IO.File.Exists(filePath))
                {
                    filePaths.Add(filePath);
                }
            }

            if (filePaths.Count == 0)
            {
                throw new FileNotFoundException();
            }

            var tempFile = Path.GetTempFileName();

            using (var zipFile = System.IO.File.Create(tempFile))
            using (var zipArchive = new ZipArchive(zipFile, ZipArchiveMode.Create))
            {
                foreach (var file in filePaths)
                {
                    zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }

            var stream = new FileStream(tempFile, FileMode.Open);

            return File(stream, "application/zip", "images.zip");
        }

    }

    public class UploadSetting
    {
        public string Session { get; set; }
    }
}
