using DTO;

namespace CarRentalAPI.Models.File
{
    public class PostFileModel
    {
        public IFormFile File { get; set; }
        public string CarName { get; set; }
        public int OrderId { get; set; }
        public FileType FileType { get; set; }
    }
}
