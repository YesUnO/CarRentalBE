namespace Core.Domain.Payment.Options;

public class StripeSettings
{
    public string EndpointSecret { get; set; }
    public string ApiKey { get; set; }
    public string CarRentalPriceId { get; set; }
}
