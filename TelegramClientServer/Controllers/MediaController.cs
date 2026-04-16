using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public MediaController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("UploadMedia")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No File");

                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";

                string subFolder = extension == ".mp4" || extension== ".mov"
                    ? "Videos" 
                    : extension == ".gif" || extension == ".gifif" ? "GIFs" 
                    : "Images";

                var uploadsPath = Path.Combine(_env.WebRootPath, "Uploads", subFolder);

                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fullPath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativeUrl = $"/Uploads/{subFolder}/{fileName}";
                return Ok(new { url = relativeUrl });
        }

        [HttpGet("GetPath/{fileName}")]
        public IActionResult GetPath(string fileName)
        {
            List<string> subFolders = new List<string> { "Images", "Videos", "GIFs" };

            foreach (var folder in subFolders)
            {
                var pathToCheck = Path.Combine(_env.WebRootPath, "Uploads", folder, fileName);

                if (System.IO.File.Exists(pathToCheck))
                {
                    return Ok(new { url = $"/Uploads/{folder}/{fileName}" });
                }
            }

            return NotFound("Bruh moment...");
        }
    }
}
