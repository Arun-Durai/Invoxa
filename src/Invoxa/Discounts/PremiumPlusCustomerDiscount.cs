using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Discounts;

public sealed class PremiumPlusCustomerDiscount : IDiscountPolicy
{
    private const decimal Rate = 0.15m;

    public string Id => "premium-plus-customer";
    public string Label => "Premium+ member (15% off)";

    public bool IsApplicable(PricingContext context) =>
        context.Customer.Type == CustomerType.PremiumPlus;

    public decimal Calculate(PricingContext context) =>
        Money.Round(context.Subtotal * Rate);
}
