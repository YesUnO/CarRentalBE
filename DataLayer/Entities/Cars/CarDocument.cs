using AutoMapper.Configuration.Annotations;
using DataLayer.Entities.Files;

namespace DataLayer.Entities.Cars
{
    public class CarDocument
    {
        public int Id { get; set; }
        public CarDocumentType CarDocumentType { get; set; }
        public int FrontSideImageId { get; set; }
        [Ignore]
        public Image FrontSideImage { get; set; }
        public int BackSideImageId { get; set; }
        [Ignore]
        public Image BackSideImage { get; set; }
        public string DocNr { get; set; }
        public DateTime ValidTill { get; set; }
        public bool IsActive { get; set; }

    }
}
