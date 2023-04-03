
using Core.Infrastructure.StripePayment.Options;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.StripePayment;

public class StripeService : IStripeService
{
    private readonly StripeSettings _stripeSettings;

    public StripeService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
    }

    public void Test()
    {
        var yo = "?";
    }
}
