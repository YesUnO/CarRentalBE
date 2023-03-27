using DataLayer.Entities.Files;
using DTO;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Files
{
    public interface IFileService
    {
        Task SaveFileAsync(IFormFile file, string carName, FileType fileType);
    }
}
