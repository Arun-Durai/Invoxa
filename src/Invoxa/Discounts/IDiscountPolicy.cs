using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Discounts;

public interface IDiscountPolicy
{
    string Id { get; }
    string Label { get; }
    bool IsApplicable(PricingContext context);
    decimal Calculate(PricingContext context);
}
