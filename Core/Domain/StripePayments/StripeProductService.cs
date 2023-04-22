using Core.Domain.StripePayments.Interfaces;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Core.Domain.StripePayments;

public class StripeProductService : IStripeProductService
{
    private readonly ILogger<StripeProductService> _logger;

    public StripeProductService(ILogger<StripeProductService> logger)
    {
        _logger = logger;
    }

    public string CreateStripeProductForCar(int price, string name)
    {
        var priceService = new PriceService();
        var options = new PriceCreateOptions
        {
            BillingScheme = "per_unit",
            UnitAmount = price * 100,
            Currency = "czk",
            ProductData = new PriceProductDataOptions
            {
                Name = name + "_car",
            }
        };
        var stripePrice = priceService.Create(options);
        var result = stripePrice.Id;
        return result;
    }
}
