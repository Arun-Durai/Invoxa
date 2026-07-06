using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Discounts;

public sealed class DecemberSeasonalDiscount : IDiscountPolicy
{
    private const decimal Rate = 0.20m;

    public string Id => "december-seasonal";
    public string Label => "December seasonal (20% off)";
    public bool IsApplicable(PricingContext context) => context.AsOf.Month == 12;
    public decimal Calculate(PricingContext context) => Money.Round(context.Subtotal * Rate);
}
