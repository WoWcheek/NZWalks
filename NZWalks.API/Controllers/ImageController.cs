using Microsoft.AspNetCore.Mvc;

using NZWalks.API.Models.DTO;
using NZWalks.API.Models.Domain;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly IImageRepository imageRepository;

    public ImageController(IImageRepository imageRepository)
    {
        this.imageRepository = imageRepository;
    }

    [HttpPost]
    [Route("Upload")]
    public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
    {
        ValidateFileUpload(request);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var imageDomain = new Image
        {
            File = request.File,
            FileName = request.FileName,
            FileSizeInBytes = request.File.Length,
            FileDescription = request.FileDescription,
            FileExtension = Path.GetExtension(request.File.FileName)
        };

        imageDomain = await imageRepository.Upload(imageDomain);

        return Ok(imageDomain);
    }

    private void ValidateFileUpload(ImageUploadRequestDto request)
    {
        var alloweExtension = new string[] { ".jpg", ".jpeg", ".png" };
    
        if (!alloweExtension.Contains(Path.GetExtension(request.File.FileName)))
        {
            ModelState.AddModelError("file", "Unsupported file extension");
        }

        const int BYTES_PER_MEGABYTE = 1048576;
        const int MAX_FILE_SIZE_IN_MEGABYTES = 10;

        if (request.File.Length > MAX_FILE_SIZE_IN_MEGABYTES * BYTES_PER_MEGABYTE)
        {
            ModelState.AddModelError("file", $"File size is bigger than {MAX_FILE_SIZE_IN_MEGABYTES}MB");
        }
    }
}
