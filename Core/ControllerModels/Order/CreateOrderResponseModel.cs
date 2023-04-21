using Core.ControllerModels.Car;

namespace Core.ControllerModels.Order;

public class CreateOrderResponseModel
{
    public int Id { get; set; }
    public CarDTO Car {get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long Price { get; set; }
    public bool Paid { get; set; } = false;
}
