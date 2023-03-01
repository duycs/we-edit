using Application.Models;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

namespace Houzz.Api.Controllers.WebControllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        [HttpPost, DisableRequestSizeLimit]
        [Route("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = FileExtension.GetImageWatermarkFolderTemp();
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
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
        public async Task<IActionResult> Download([FromQuery] string fileUrl)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);

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
        public async Task<IActionResult> Remove([FromQuery] string fileUrl)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            System.IO.File.Delete(fileUrl);
            return NoContent();
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("photos")]
        public IActionResult GetPhotos()
        {
            try
            {
                var folderName = FileExtension.GetImageWatermarkFolder();
                var pathToRead = Path.Combine(Directory.GetCurrentDirectory(), folderName);
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
    }
}
