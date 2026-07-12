using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Shipping;
using Invoxa.Tax;

namespace Invoxa.Pricing;

public sealed class InvoiceCalculator
{
    private readonly DiscountEngine _discountEngine;
    private readonly ITaxPolicy _taxPolicy;
    private readonly IShippingPolicy _shippingPolicy;

    public InvoiceCalculator(
        DiscountEngine discountEngine,
        ITaxPolicy taxPolicy,
        IShippingPolicy shippingPolicy)
    {
        _discountEngine = discountEngine;
        _taxPolicy = taxPolicy;
        _shippingPolicy = shippingPolicy;
    }

    public Invoice Calculate(Cart cart, DateTime asOf)
    {
        var subtotal = SubtotalCalculator.Calculate(cart.LineItems);
        var context = new PricingContext(cart.Customer, cart.LineItems, subtotal, asOf);

        var discount = _discountEngine.Resolve(context);
        var discountAmount = discount?.Amount ?? 0m;
        var taxableBase = Money.Round(subtotal - discountAmount);

        var tax = _taxPolicy.Calculate(taxableBase, context);
        var shipping = _shippingPolicy.Calculate(cart.Customer, context);
        var grandTotal = Money.Round(taxableBase + tax.Amount + shipping.Amount);

        var lines = cart.LineItems
            .Select(item => new InvoiceLine(item.Name, item.UnitPrice, item.Quantity, item.LineTotal))
            .ToList();

        return new Invoice(
            invoiceNumber: string.Empty,
            cart.Customer,
            lines,
            subtotal,
            discount,
            tax,
            shipping,
            grandTotal,
            asOf);
    }
}
