using DTO;

namespace CarRentalAPI.Models.File;

public class GetUserDocumentPhotoModel
{
    public string Mail { get; set; }
    public UserDocumentImageType UserDocumentImageType { get; set; }
}
