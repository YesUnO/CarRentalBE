using DataLayer.Entities.Files;

namespace DataLayer.Entities.Cars
{
    public class CarDocument
    {
        public int Id { get; set; }
        public CarDocumentType CarDocumentType { get; set; }
        public Image FrontSide { get; set; }
        public Image BackSide { get; set; }
        public string DocNr { get; set; }
        public DateTime ValidTill { get; set; }
        public bool IsActive { get; set; }

    }
}
