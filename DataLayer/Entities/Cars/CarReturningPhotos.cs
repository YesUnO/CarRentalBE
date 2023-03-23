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

        public CarReturningPhotos(Image frontSide,
                                  Image backSide,
                                  Image side,
                                  Image otherSide,
                                  Image trunk,
                                  Image cabine,
                                  Image dashboard)
        {
            CreatedAt = DateTime.Now;
            FrontSide = frontSide;
            BackSide = backSide;
            Side = side;
            OtherSide = otherSide;
            Trunk = trunk;
            Cabine = cabine;
            Dashboard = dashboard;
        }
    }
}
