using DTO;

namespace CarRentalAPI.Models.File
{
    public class GetUserDocumentPhotoModel
    {
        public string UserName { get; set; }
        public UserDocumentImageType UserDocumentImageType { get; set; }
    }
}
