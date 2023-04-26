using DTO;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Files;

public interface IFileService
{
    Task SaveCarReturningPhotoAsync(IFormFile file, int orderId, CarReturningImageType carReturningImageType, string loggedinUserMail);
    Task SaveUserDocumentPhotoAsync(IFormFile file, UserDocumentImageType imageType, string loggedinUserMail);
    Task<FileStream> GetUserDocumentPhoto(string mail, UserDocumentImageType userDocumentImageType);
    Task SaveCarProfilePickAsync(int carId, IFormFile file);
}
