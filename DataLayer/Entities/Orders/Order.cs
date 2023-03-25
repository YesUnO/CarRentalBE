using DataLayer.Entities.Cars;
using DataLayer.Entities.Files;
using DataLayer.Entities.User;

namespace DataLayer.Entities.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public ApplicationUser Customer { get; set; }
        public Car Car { get; set; }
        public Payment? Payment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CancelledAt { get; set; }
        public Accident? Accident { get; set; }
        public decimal Distance { get; set; }
        public List<Image>? ReturningPhotos { get; set; }
    }
}
