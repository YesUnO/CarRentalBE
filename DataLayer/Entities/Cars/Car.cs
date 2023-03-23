using DataLayer.Entities.Files;

namespace DataLayer.Entities.Cars
{
    public class Car
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public CarSpecification? CarSpecification { get; set; }
        public Image? ProfilePic { get; set; }
        public CarDocument? STK { get; set; }
        public CarDocument? TechnicLicense { get; set; }
        public DateTime PurchasedAt { get; set; }
        public List<PDF>? PurchaseContract { get; set; }
        public decimal MileageAtPurchase { get; set; }
        public decimal CurrentMileage { get; set; }
        public CarInsurance? CarInsurance { get; set; }
        public CarState CarState { get; set; }
        public CarServiceState CarServiceState { get;set;}
        public decimal BasicRentalPrice { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
