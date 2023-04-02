using DTO;

namespace CarRentalAPI.Models.File
{
    public class PostCarReturningPhotoModel
    {
        public IFormFile File { get; set; }
        public CarReturningImageType CarReturningImageType { get; set; }
    }
}
