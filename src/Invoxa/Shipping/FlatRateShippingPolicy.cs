using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Shipping;

public sealed class FlatRateShippingPolicy : IShippingPolicy
{
    public const decimal FlatRate = 50m;

    public ShippingLine Calculate(Customer customer, PricingContext context)
    {
        if (customer.Type == CustomerType.PremiumPlus)
            return new ShippingLine("Free shipping", 0m);

        return new ShippingLine("Shipping", FlatRate);
    }
}
