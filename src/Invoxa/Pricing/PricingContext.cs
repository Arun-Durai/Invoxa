using Invoxa.Domain;

namespace Invoxa.Pricing;

public sealed class PricingContext
{
    public Customer Customer { get; }
    public IReadOnlyList<LineItem> LineItems { get; }
    public decimal Subtotal { get; }
    public DateTime AsOf { get; }

    public PricingContext(
        Customer customer,
        IReadOnlyList<LineItem> lineItems,
        decimal subtotal,
        DateTime asOf)
    {
        Customer = customer;
        LineItems = lineItems;
        Subtotal = subtotal;
        AsOf = asOf;
    }
}
