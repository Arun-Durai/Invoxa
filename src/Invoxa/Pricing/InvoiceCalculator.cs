using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Tax;

namespace Invoxa.Pricing;

public sealed class InvoiceCalculator
{
    private readonly DiscountEngine _discountEngine;
    private readonly ITaxPolicy _taxPolicy;

    public InvoiceCalculator(DiscountEngine discountEngine, ITaxPolicy taxPolicy)
    {
        _discountEngine = discountEngine;
        _taxPolicy = taxPolicy;
    }

    public Invoice Calculate(Cart cart, DateTime asOf)
    {
        var subtotal = SubtotalCalculator.Calculate(cart.LineItems);
        var context = new PricingContext(cart.Customer, cart.LineItems, subtotal, asOf);

        var discount = _discountEngine.Resolve(context);
        var discountAmount = discount?.Amount ?? 0m;
        var taxableBase = Money.Round(subtotal - discountAmount);

        var tax = _taxPolicy.Calculate(taxableBase, context);
        var grandTotal = Money.Round(taxableBase + tax.Amount);

        var lines = cart.LineItems
            .Select(item => new InvoiceLine(item.Name, item.UnitPrice, item.Quantity, item.LineTotal))
            .ToList();

        return new Invoice(
            cart.Customer,
            lines,
            subtotal,
            discount,
            tax,
            grandTotal,
            asOf);
    }
}
