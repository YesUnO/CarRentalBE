
namespace Core.Infrastructure.StripePayment;

public interface IStripeService
{
    void Test();
    string CheckCheckoutSession(string feUrl);
}
