using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Tax;

public sealed class FlatRateTaxPolicy(decimal rate, string label = "Sales tax") : ITaxPolicy
{
    public TaxLine Calculate(decimal taxableBase, PricingContext context)
    {
        var amount = Money.Round(taxableBase * rate);
        return new TaxLine(label, rate, amount);
    }
}
