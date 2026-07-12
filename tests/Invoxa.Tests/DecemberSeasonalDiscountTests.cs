using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Pricing;
using Invoxa.Tax;

namespace Invoxa.Tests;

public class DecemberSeasonalDiscountTests
{
    private static readonly DateTime DecemberDate = new(2026, 12, 15, 10, 0, 0);
    private static readonly DateTime JanuaryDate = new(2026, 1, 15, 10, 0, 0);

    private static PricingContext CreateContext(
        CustomerType customerType,
        decimal subtotal,
        DateTime asOf)
    {
        var lineItems = new List<LineItem> { new("Item", subtotal, 1) };
        var customer = new Customer("Test", customerType);
        return new PricingContext(customer, lineItems, subtotal, asOf);
    }

    private static DiscountEngine CreateEngine() =>
        new(
        [
            new PremiumCustomerDiscount(),
            new DecemberSeasonalDiscount()
        ]);

    private static InvoiceCalculator CreateCalculator() =>
        TestFactory.CreateCalculator(
        [
            new PremiumCustomerDiscount(),
            new DecemberSeasonalDiscount()
        ]);

    [Fact]
    public void IsApplicable_InDecember_ReturnsTrue()
    {
        var policy = new DecemberSeasonalDiscount();
        var context = CreateContext(CustomerType.Regular, 100m, DecemberDate);

        Assert.True(policy.IsApplicable(context));
        Assert.Equal(20m, policy.Calculate(context));
    }

    [Fact]
    public void IsApplicable_OutsideDecember_ReturnsFalse()
    {
        var policy = new DecemberSeasonalDiscount();
        var context = CreateContext(CustomerType.Regular, 100m, JanuaryDate);

        Assert.False(policy.IsApplicable(context));
    }

    [Fact]
    public void Resolve_RegularCustomerInDecember_AppliesTwentyPercentOff()
    {
        var engine = CreateEngine();
        var context = CreateContext(CustomerType.Regular, 200m, DecemberDate);

        var result = engine.Resolve(context);

        Assert.NotNull(result);
        Assert.Equal("december-seasonal", result.PolicyId);
        Assert.Equal(40m, result.Amount);
    }

    [Fact]
    public void Resolve_RegularCustomerInJanuary_NoDiscount()
    {
        var engine = CreateEngine();
        var context = CreateContext(CustomerType.Regular, 200m, JanuaryDate);

        var result = engine.Resolve(context);

        Assert.Null(result);
    }

    [Fact]
    public void Resolve_PremiumCustomerInDecember_SeasonalWinsOverPremium()
    {
        // Premium 10% = ₹20; December 20% = ₹40 on ₹200 subtotal.
        var engine = CreateEngine();
        var context = CreateContext(CustomerType.Premium, 200m, DecemberDate);

        var result = engine.Resolve(context);

        Assert.NotNull(result);
        Assert.Equal("december-seasonal", result.PolicyId);
        Assert.Equal(40m, result.Amount);
    }

    [Fact]
    public void InvoiceCalculator_DecemberSeasonal_AppliesOnInvoice()
    {
        var cart = new Cart(
            new Customer("Regular User", CustomerType.Regular),
            [new LineItem("Widget", 100m, 2)]);

        var invoice = CreateCalculator().Calculate(cart, DecemberDate);

        Assert.Equal(200m, invoice.Subtotal);
        Assert.NotNull(invoice.Discount);
        Assert.Equal("december-seasonal", invoice.Discount.PolicyId);
        Assert.Equal(40m, invoice.Discount.Amount);
        Assert.Equal(12.80m, invoice.Tax.Amount);
        Assert.Equal(50m, invoice.Shipping.Amount);
        Assert.Equal(222.80m, invoice.GrandTotal);
    }
}
