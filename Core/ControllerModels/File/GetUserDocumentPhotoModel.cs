using DTO;

namespace Core.ControllerModels.File;

public class GetUserDocumentPhotoModel
{
    public string Mail { get; set; }
    public UserDocumentImageType UserDocumentImageType { get; set; }
}
