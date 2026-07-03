using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Discounts;

public sealed class PremiumCustomerDiscount : IDiscountPolicy
{
    private const decimal Rate = 0.10m;

    public string Id => "premium-customer";
    public string Label => "Premium member (10% off)";

    public bool IsApplicable(PricingContext context) =>
        context.Customer.Type == CustomerType.Premium;

    public decimal Calculate(PricingContext context) =>
        Money.Round(context.Subtotal * Rate);
}
