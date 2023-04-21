namespace Core.ControllerModels.Car;

public class CreateCarRequestModel
{
    public string Name { get; set; }
    public decimal MileageAtPurchase { get; set; }
    public decimal PurchasePrice { get; set; }
}
