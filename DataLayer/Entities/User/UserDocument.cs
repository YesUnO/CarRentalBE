using DataLayer.Entities.Files;

namespace DataLayer.Entities.User
{
    public class UserDocument
    {
        public int Id { get; set; }
        public UserDocumentType UserDocumentType { get; set; }
        public UserDocumentImage FrontSideImage { get; set; }
        public UserDocumentImage BackSideImage {get; set;}
        public bool Checked { get; set; }
        public string? DocNr { get; set; }
        public DateTime ValidTill { get; set; }
        public bool IsActive { get; set; }
        public List<ApplicationUser> DriverLicenseUsers { get; set; }
        public List<ApplicationUser> IdentificationLicenseUsers { get; set; }

    }
}
