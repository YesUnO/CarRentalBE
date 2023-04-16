using DTO;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Files;

public interface IFileService
{
    Task SaveCarReturningPhotoAsync(IFormFile file, int orderId, CarReturningImageType carReturningImageType);
    Task SaveUserDocumentPhotoAsync(IFormFile file, UserDocumentImageType imageType);
    Task<FileStream> GetUserDocumentPhoto(string mail, UserDocumentImageType userDocumentImageType);
}
