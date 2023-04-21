using DTO;
using Microsoft.AspNetCore.Http;

namespace Core.ControllerModels.File;

public class PostCarReturningPhotoModel
{
    public IFormFile File { get; set; }
    public CarReturningImageType CarReturningImageType { get; set; }
}
