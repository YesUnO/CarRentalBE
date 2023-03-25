using DataLayer.Entities.Files;
using DataLayer.Entities.User;

namespace DataLayer.Entities.Cars
{
    public class CarDocument
    {
        public int Id { get; set; }
        public CarDocumentType CarDocumentType { get; set; }
        public CarDocumentImage FrontSideImage { get; set; }
        public CarDocumentImage BackSideImage { get; set; }
        public string DocNr { get; set; }
        public DateTime ValidTill { get; set; }
        public bool IsActive { get; set; }
        public List<Car> STKUsers { get; set; }
        public List<Car> TechnicLicenseUsers { get; set; }
    }
}
