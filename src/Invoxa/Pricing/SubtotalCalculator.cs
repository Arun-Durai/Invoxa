using Invoxa.Domain;

namespace Invoxa.Pricing;

public static class SubtotalCalculator
{
    public static decimal Calculate(IReadOnlyList<LineItem> lineItems) =>
        Money.Round(lineItems.Sum(item => item.LineTotal));
}
