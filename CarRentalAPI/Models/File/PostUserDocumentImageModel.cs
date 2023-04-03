using DTO;

namespace CarRentalAPI.Models.File;

public class PostUserDocumentImageModel
{
    public IFormFile File { get; set; }
    public UserDocumentImageType UserDocumentImageType { get; set; }
}
