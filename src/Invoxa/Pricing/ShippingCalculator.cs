using Invoxa.Domain;

namespace Invoxa.Pricing;

public static class ShippingCalculator
{
    public const decimal FlatRate = 50m;

    public static ShippingLine Calculate(Customer customer)
    {
        if (customer.Type == CustomerType.PremiumPlus)
            return new ShippingLine("Free shipping", 0m);

        return new ShippingLine("Shipping", FlatRate);
    }
}
