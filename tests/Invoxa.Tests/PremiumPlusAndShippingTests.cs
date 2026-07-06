using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Pricing;
using Invoxa.Tax;

namespace Invoxa.Tests;

public class PremiumPlusAndShippingTests
{
    private static readonly DateTime FixedDate = new(2026, 7, 3, 10, 0, 0);

    private static InvoiceCalculator CreateCalculator() =>
        new(
            new DiscountEngine(
            [
                new PremiumCustomerDiscount(),
                new PremiumPlusCustomerDiscount(),
                new DecemberSeasonalDiscount()
            ]),
            new FlatRateTaxPolicy(0.08m));

    [Fact]
    public void RegularCustomer_PaysFlatShippingFee()
    {
        var cart = new Cart(
            new Customer("Regular", CustomerType.Regular),
            [new LineItem("Item", 100m, 1)]);

        var invoice = CreateCalculator().Calculate(cart, FixedDate);

        Assert.Equal("Shipping", invoice.Shipping.Label);
        Assert.Equal(50m, invoice.Shipping.Amount);
        Assert.Equal(158m, invoice.GrandTotal);
    }

    [Fact]
    public void PremiumPlusCustomer_GetsFifteenPercentOff_AndFreeShipping()
    {
        var cart = new Cart(
            new Customer("Premium Plus", CustomerType.PremiumPlus),
            [new LineItem("Item", 200m, 2)]);

        var invoice = CreateCalculator().Calculate(cart, FixedDate);

        Assert.NotNull(invoice.Discount);
        Assert.Equal("premium-plus-customer", invoice.Discount.PolicyId);
        Assert.Equal(60m, invoice.Discount.Amount);
        Assert.Equal("Free shipping", invoice.Shipping.Label);
        Assert.Equal(0m, invoice.Shipping.Amount);
        Assert.Equal(27.20m, invoice.Tax.Amount);
        Assert.Equal(367.20m, invoice.GrandTotal);
    }
}
