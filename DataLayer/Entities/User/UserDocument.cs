using DataLayer.Entities.Files;

namespace DataLayer.Entities.User
{
    public class UserDocument
    {
        public int Id { get; set; }
        public UserDocumentType UserDocumentType { get; set; }
        public Image FrontSide { get; set; }
        public Image BackSide { get; set; }
        public bool Checked { get; set; }
        public string DocNr { get; set; }
        public DateTime ValidTill { get; set; }
        public bool IsActive { get; set; }

    }
}
