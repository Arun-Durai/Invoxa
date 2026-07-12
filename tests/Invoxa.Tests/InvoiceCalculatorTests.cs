using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Pricing;
using Invoxa.Tax;

namespace Invoxa.Tests;

public class InvoiceCalculatorTests
{
    private static readonly DateTime FixedDate = new(2026, 7, 3, 10, 0, 0);

    [Fact]
    public void RegularCustomer_NoDiscount_TaxOnSubtotal_PaysShipping()
    {
        var cart = new Cart(
            new Customer("Test User", CustomerType.Regular),
            [new LineItem("Widget", 100m, 2)]);

        var invoice = TestFactory.CreateCalculator().Calculate(cart, FixedDate);

        Assert.Equal(200m, invoice.Subtotal);
        Assert.Null(invoice.Discount);
        Assert.Equal(16m, invoice.Tax.Amount);
        Assert.Equal(50m, invoice.Shipping.Amount);
        Assert.Equal(266m, invoice.GrandTotal);
    }

    [Fact]
    public void PremiumCustomer_GetsTenPercentDiscount_TaxOnDiscountedAmount_PaysShipping()
    {
        var cart = new Cart(
            new Customer("Premium User", CustomerType.Premium),
            [new LineItem("Widget", 100m, 2)]);

        var invoice = TestFactory.CreateCalculator().Calculate(cart, FixedDate);

        Assert.Equal(200m, invoice.Subtotal);
        Assert.NotNull(invoice.Discount);
        Assert.Equal(20m, invoice.Discount.Amount);
        Assert.Equal(14.40m, invoice.Tax.Amount);
        Assert.Equal(50m, invoice.Shipping.Amount);
        Assert.Equal(244.40m, invoice.GrandTotal);
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

        var invoice = TestFactory.CreateCalculator().Calculate(cart, FixedDate);

        Assert.Equal(190m, invoice.Subtotal);
        Assert.Equal(2, invoice.Lines.Count);
        Assert.Equal(255.20m, invoice.GrandTotal);
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
