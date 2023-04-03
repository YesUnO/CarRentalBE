
using System.Globalization;

namespace DataLayer.Entities.Cars;

public class CarSpecification
{
    public int Id { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal TrunkLength { get; set; }
    public decimal TrunkWidth { get; set; }
    public decimal TrunkHeight { get; set; }
    public decimal TrunkVolume { get; set; }
    public decimal LoadCapacity { get; set; }
    public int NumberOfSeats { get; set; }
    public DateTime ManufacturedAt { get; set; }

    //TODO: add helper to parse regioninfo i guess
    public string ManufacturedIn { get; set; }
}
