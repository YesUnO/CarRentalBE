using DataLayer.Entities.Cars;
using DataLayer.Entities.Orders;

namespace DataLayer.Entities.Files
{
    public class PDF
    {
        public int Id { get; set; }
        public string RelativePath { get; set; }
        public Car? Car { get; set; }
        public CarInsurance? CarInsurance { get; set;}
        public Accident? Accident { get; set; }
    }
}
