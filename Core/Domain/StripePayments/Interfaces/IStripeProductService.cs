
namespace Core.Domain.StripePayments.Interfaces;

public interface IStripeProductService
{
    string CreateStripeProductForCar(int price, string name);
}
