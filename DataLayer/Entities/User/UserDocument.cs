using DataLayer.Entities.Files;

namespace DataLayer.Entities.User
{
    public class UserDocument
    {
        public int Id { get; set; }
        public UserDocumentType UserDocumentType { get; set; }
        public int FrontSideImageId { get; set; }
        public Image FrontSideImage { get; set; }
        public int BackSideImageId { get; set; }
        public Image BackSideImage {get; set;}
        public bool Checked { get; set; }
        public string DocNr { get; set; }
        public DateTime ValidTill { get; set; }
        public bool IsActive { get; set; }

    }
}
