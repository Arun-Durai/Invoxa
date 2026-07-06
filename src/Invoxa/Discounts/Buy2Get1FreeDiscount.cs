using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Discounts;

public sealed class Buy2Get1FreeDiscount : IDiscountPolicy
{
    private readonly HashSet<string> _qualifyingProductIds;

    public Buy2Get1FreeDiscount(IEnumerable<string> qualifyingProductIds)
    {
        _qualifyingProductIds = qualifyingProductIds.ToHashSet(StringComparer.Ordinal);
    }

    public string Id => "buy-2-get-1-free";
    public string Label => "Buy 2 Get 1 Free";

    public bool IsApplicable(PricingContext context) => Calculate(context) > 0;

    public decimal Calculate(PricingContext context)
    {
        decimal total = 0m;

        foreach (var item in context.LineItems)
        {
            if (item.ProductId is null || !_qualifyingProductIds.Contains(item.ProductId))
                continue;

            var freeUnits = item.Quantity / 3;
            if (freeUnits > 0)
                total += freeUnits * item.UnitPrice;
        }

        return Money.Round(total);
    }
}
