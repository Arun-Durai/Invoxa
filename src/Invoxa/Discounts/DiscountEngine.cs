using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Discounts;

public sealed class DiscountEngine
{
    private readonly IReadOnlyList<IDiscountPolicy> _policies;

    public DiscountEngine(IEnumerable<IDiscountPolicy> policies)
    {
        _policies = policies.ToList();
    }

    /// <summary>
    /// Evaluates all registered discount policies and returns the single best offer.
    /// When multiple policies apply, only the one with the highest monetary amount is used — discounts never stack.
    /// If two amounts are equal, the policy registered first in the list wins.
    /// </summary>
    public AppliedDiscount? Resolve(PricingContext context)
    {
        AppliedDiscount? best = null;

        foreach (var policy in _policies)
        {
            if (!policy.IsApplicable(context))
                continue;

            var amount = Money.Round(policy.Calculate(context));
            if (amount <= 0)
                continue;

            if (best is null || amount > best.Amount)
            {
                best = new AppliedDiscount(policy.Id, policy.Label, amount);
            }
        }

        return best;
    }
}
