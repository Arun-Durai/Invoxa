using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Tax;

public interface ITaxPolicy
{
    TaxLine Calculate(decimal taxableBase, PricingContext context);
}
