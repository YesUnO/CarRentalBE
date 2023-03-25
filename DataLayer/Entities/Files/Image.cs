using DataLayer.Entities.Cars;
using DataLayer.Entities.Orders;
using DataLayer.Entities.User;

namespace DataLayer.Entities.Files
{
    public class Image
    {
        public int Id { get; set; }
        public ImageType ImageType { get; set; }
        public string RelativePath { get; set; }
        public Order? Order { get; set; }
        public int? UserDocumentId { get; set; }
        public List<UserDocument>? UserDocuments { get; set; }
        public int? CarDocumentId { get; set; }
        public List<CarDocument>? CarDocuments { get; set; }
        public Car? Car { get; set; }
        public Accident? Accident { get; set; }
    }
}
