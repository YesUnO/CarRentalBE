namespace Core.ControllerModels.Order;

public class CreateOrderRequestModel
{
    public int CarId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
