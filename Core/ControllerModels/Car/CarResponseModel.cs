
namespace Core.ControllerModels.Car;

public class CarDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<DateTime> Unavailable { get; set; }
    public string PictureUrl { get; set; }
}
