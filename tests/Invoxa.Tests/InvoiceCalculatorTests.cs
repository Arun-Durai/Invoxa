using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Pricing;
using Invoxa.Tax;

namespace Invoxa.Tests;

public class InvoiceCalculatorTests
{
    private static readonly DateTime FixedDate = new(2026, 7, 3, 10, 0, 0);

    private static InvoiceCalculator CreateCalculator() =>
        new(
            new DiscountEngine([new PremiumCustomerDiscount()]),
            new FlatRateTaxPolicy(0.08m));

    [Fact]
    public void RegularCustomer_NoDiscount_TaxOnSubtotal()
    {
        var cart = new Cart(
            new Customer("Test User", CustomerType.Regular),
            [new LineItem("Widget", 100m, 2)]);

        var invoice = CreateCalculator().Calculate(cart, FixedDate);

        Assert.Equal(200m, invoice.Subtotal);
        Assert.Null(invoice.Discount);
        Assert.Equal(16m, invoice.Tax.Amount);
        Assert.Equal(216m, invoice.GrandTotal);
    }

    [Fact]
    public void PremiumCustomer_GetsTenPercentDiscount_TaxOnDiscountedAmount()
    {
        var cart = new Cart(
            new Customer("Premium User", CustomerType.Premium),
            [new LineItem("Widget", 100m, 2)]);

        var invoice = CreateCalculator().Calculate(cart, FixedDate);

        Assert.Equal(200m, invoice.Subtotal);
        Assert.NotNull(invoice.Discount);
        Assert.Equal(20m, invoice.Discount.Amount);
        Assert.Equal(14.40m, invoice.Tax.Amount);
        Assert.Equal(194.40m, invoice.GrandTotal);
    }

    [Fact]
    public void MultipleLineItems_SubtotalIsSumOfLineTotals()
    {
        var cart = new Cart(
            new Customer("Test User", CustomerType.Regular),
            [
                new LineItem("Item A", 50m, 2),
                new LineItem("Item B", 30m, 3)
            ]);

        var invoice = CreateCalculator().Calculate(cart, FixedDate);

        Assert.Equal(190m, invoice.Subtotal);
        Assert.Equal(2, invoice.Lines.Count);
        Assert.Equal(205.20m, invoice.GrandTotal);
    }

    [Fact]
    public void PremiumDiscountPolicy_OnlyAppliesToPremiumCustomers()
    {
        var policy = new PremiumCustomerDiscount();
        var context = new PricingContext(
            new Customer("Regular", CustomerType.Regular),
            [new LineItem("Item", 100m, 1)],
            100m,
            FixedDate);

        Assert.False(policy.IsApplicable(context));

        var premiumContext = new PricingContext(
            new Customer("Premium", CustomerType.Premium),
            context.LineItems,
            context.Subtotal,
            context.AsOf);

        Assert.Equal(10m, policy.Calculate(premiumContext));
    }
}
