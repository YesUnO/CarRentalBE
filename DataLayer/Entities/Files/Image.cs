using DataLayer.Entities.Cars;
using DataLayer.Entities.Orders;
using DataLayer.Entities.User;

namespace DataLayer.Entities.Files
{
    public class Image
    {
        public int Id { get; set; }
        public string RelativePath { get; set; }
    }

    public class OrderImage : Image
    {
        public CarImageType CarImageType { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class CarDocumentImage: Image
    {
        public List<CarDocument> FrontCarDocuments { get; set; }
        public List<CarDocument> BackCarDocuments { get; set; }
    }

    public class UserDocumentImage : Image
    {
        public List<UserDocument> FrontUserDocuments { get; set; }
        public List<UserDocument> BackUserDocuments { get; set; }
    }

    public class AccidentImage: Image
    {
        public List<Accident> Accidents { get; set; }
    }
}
