using DataLayer.Entities.Files;

namespace DataLayer.Entities.Cars
{
    public class CarReturningPhotos
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Image FrontSide { get; set; }
        public Image BackSide { get; set; }
        public Image Side { get; set; }
        public Image OtherSide { get; set; }
        public Image Trunk { get; set; }
        public Image Cabine { get; set; }
        public Image Dashboard { get; set; }

    }
}
