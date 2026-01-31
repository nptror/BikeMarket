using Microsoft.AspNetCore.Http;

namespace Business.Interface;

public interface IPhotoService
{
    Task<string> UploadImageAsync(IFormFile file);
}
