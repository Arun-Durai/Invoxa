using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Pricing;
using Invoxa.Tax;

namespace Invoxa.Tests;

public class Buy2Get1FreeDiscountTests
{
    private const string PromoProductId = "PROMO-B2G1";
    private static readonly DateTime FixedDate = new(2026, 7, 3, 10, 0, 0);

    private static Buy2Get1FreeDiscount CreatePolicy() =>
        new([PromoProductId]);

    [Fact]
    public void Calculate_QuantityTwo_NoFreeUnits()
    {
        var policy = CreatePolicy();
        var context = CreateContext([new LineItem("Mug", 300m, 2, PromoProductId)], 600m);

        Assert.False(policy.IsApplicable(context));
        Assert.Equal(0m, policy.Calculate(context));
    }

    [Fact]
    public void Calculate_QuantityThree_OneFreeUnit()
    {
        var policy = CreatePolicy();
        var context = CreateContext([new LineItem("Mug", 300m, 3, PromoProductId)], 900m);

        Assert.True(policy.IsApplicable(context));
        Assert.Equal(300m, policy.Calculate(context));
    }

    [Fact]
    public void Calculate_QuantitySix_TwoFreeUnits()
    {
        var policy = CreatePolicy();
        var context = CreateContext([new LineItem("Mug", 300m, 6, PromoProductId)], 1800m);

        Assert.Equal(600m, policy.Calculate(context));
    }

    [Fact]
    public void Calculate_NonQualifyingItem_NoDiscount()
    {
        var policy = CreatePolicy();
        var context = CreateContext([new LineItem("Pen", 100m, 3)], 300m);

        Assert.False(policy.IsApplicable(context));
    }

    [Fact]
    public void InvoiceCalculator_B2G1CompetesWithOtherDiscounts_OnlyHighestApplied()
    {
        var calculator = TestFactory.CreateCalculator(
        [
            new PremiumCustomerDiscount(),
            new Buy2Get1FreeDiscount([PromoProductId])
        ]);

        var cart = new Cart(
            new Customer("Premium User", CustomerType.Premium),
            [new LineItem("Mug", 300m, 3, PromoProductId)]);

        var invoice = calculator.Calculate(cart, FixedDate);

        Assert.NotNull(invoice.Discount);
        Assert.Equal("buy-2-get-1-free", invoice.Discount.PolicyId);
        Assert.Equal(300m, invoice.Discount.Amount);
    }

    private static PricingContext CreateContext(IReadOnlyList<LineItem> items, decimal subtotal) =>
        new(new Customer("Test", CustomerType.Regular), items, subtotal, FixedDate);
}
