using DTO;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Files
{
    public interface IFileService
    {
        Task SaveFileAsync(IFormFile file, FileType fileType, int? orderId, int? userDocumentId);
    }
}
