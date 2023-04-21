using DTO;
using Microsoft.AspNetCore.Http;

namespace Core.ControllerModels.File;

public class PostUserDocumentImageModel
{
    public IFormFile File { get; set; }
    public UserDocumentImageType UserDocumentImageType { get; set; }
}
