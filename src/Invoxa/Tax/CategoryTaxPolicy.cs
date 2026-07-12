using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Tax;

public sealed class CategoryTaxPolicy : ITaxPolicy
{
    private readonly IReadOnlyDictionary<ProductCategory, decimal> _rates;

    public CategoryTaxPolicy(IReadOnlyDictionary<ProductCategory, decimal>? rates = null)
    {
        _rates = rates ?? new Dictionary<ProductCategory, decimal>
        {
            [ProductCategory.Electronics] = 0.08m,
            [ProductCategory.Groceries] = 0.02m,
            [ProductCategory.General] = 0.08m
        };
    }

    public TaxLine Calculate(decimal taxableBase, PricingContext context)
    {
        if (context.Subtotal <= 0 || taxableBase <= 0)
            return new TaxLine("Sales tax (by category)", 0m, 0m);

        var ratio = taxableBase / context.Subtotal;
        decimal totalTax = 0m;

        foreach (var item in context.LineItems)
        {
            var rate = _rates.TryGetValue(item.Category, out var r) ? r : 0.08m;
            var lineTaxable = Money.Round(item.LineTotal * ratio);
            totalTax += Money.Round(lineTaxable * rate);
        }

        totalTax = Money.Round(totalTax);
        var effectiveRate = taxableBase == 0 ? 0m : Money.Round(totalTax / taxableBase);

        return new TaxLine("Sales tax (by category)", effectiveRate, totalTax);
    }
}
