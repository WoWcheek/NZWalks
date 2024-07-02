using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories;

public class LocalImageRepository : IImageRepository
{
    private readonly NZWalksDbContext dbContext;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;

    public LocalImageRepository(
        NZWalksDbContext dbContext,
        IWebHostEnvironment webHostEnvironment,
        IHttpContextAccessor httpContextAccessor)
    {
        this.dbContext = dbContext;
        this.webHostEnvironment = webHostEnvironment;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Image> Upload(Image image)
    {
        var localFilePath = Path.Combine(
            webHostEnvironment.ContentRootPath,
            "Images", 
            $"{image.FileName}{image.FileExtension}");

        using var stream = new FileStream(localFilePath, FileMode.Create);
        await image.File.CopyToAsync(stream);

        var httpContextRequest = httpContextAccessor.HttpContext.Request;

        var urlFilePath = 
            $"{httpContextRequest.Scheme}://" +
            $"{httpContextRequest.Host}{httpContextRequest.PathBase}/" +
            $"Images/{image.FileName}{image.FileExtension}";
    
        image.FilePath = urlFilePath;

        await dbContext.Images.AddAsync(image);
        await dbContext.SaveChangesAsync();

        return image;
    }
}
