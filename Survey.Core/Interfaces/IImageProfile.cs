using Microsoft.AspNetCore.Http;

namespace Survey.Core.Interfaces
{
    public interface IImageProfile
    {
        void UploadImage(IFormFile file);
    }
}