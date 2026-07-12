using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Pricing;
using Invoxa.Tax;

namespace Invoxa.Tests;

public class DiscountEngineTests
{
    private static readonly DateTime FixedDate = new(2026, 7, 3, 10, 0, 0);

    /// <summary>
    /// Test-only discount policy. Simulates a competing offer (e.g. future December 20% off)
    /// without adding production code for INVOXA-202 yet.
    /// </summary>
    private sealed class PercentageTestDiscount : IDiscountPolicy
    {
        private readonly decimal _rate;

        public PercentageTestDiscount(string id, string label, decimal rate)
        {
            Id = id;
            Label = label;
            _rate = rate;
        }

        public string Id { get; }
        public string Label { get; }

        public bool IsApplicable(PricingContext context) => true;

        public decimal Calculate(PricingContext context) =>
            Money.Round(context.Subtotal * _rate);
    }

    private static PricingContext CreateContext(
        CustomerType customerType,
        decimal subtotal)
    {
        var lineItems = new List<LineItem> { new("Item", subtotal, 1) };
        var customer = new Customer("Test", customerType);
        return new PricingContext(customer, lineItems, subtotal, FixedDate);
    }

    [Fact]
    public void Resolve_WhenNoPolicyApplies_ReturnsNull()
    {
        var engine = new DiscountEngine([new PremiumCustomerDiscount()]);
        var context = CreateContext(CustomerType.Regular, 100m);

        var result = engine.Resolve(context);

        Assert.Null(result);
    }

    [Fact]
    public void Resolve_WhenOnlyOnePolicyApplies_ReturnsThatDiscount()
    {
        var engine = new DiscountEngine([new PremiumCustomerDiscount()]);
        var context = CreateContext(CustomerType.Premium, 200m);

        var result = engine.Resolve(context);

        Assert.NotNull(result);
        Assert.Equal("premium-customer", result.PolicyId);
        Assert.Equal(20m, result.Amount);
    }

    [Fact]
    public void Resolve_WhenTwoPoliciesApply_PicksHighestAmount_NotStacked()
    {
        // Premium 10% on ₹200 = ₹20; test stub 20% = ₹40 → stub must win (not ₹60 stacked).
        var engine = new DiscountEngine(
        [
            new PremiumCustomerDiscount(),
            new PercentageTestDiscount("test-20-percent", "Test 20% off", 0.20m)
        ]);

        var context = CreateContext(CustomerType.Premium, 200m);

        var result = engine.Resolve(context);

        Assert.NotNull(result);
        Assert.Equal("test-20-percent", result.PolicyId);
        Assert.Equal(40m, result.Amount);
    }

    [Fact]
    public void Resolve_WhenAmountsAreEqual_FirstRegisteredPolicyWins()
    {
        // Both policies yield ₹20 on ₹200 subtotal; engine uses strict '>' so first wins.
        var first = new PercentageTestDiscount("first-10", "First 10%", 0.10m);
        var second = new PercentageTestDiscount("second-10", "Second 10%", 0.10m);

        var engine = new DiscountEngine([first, second]);
        var context = CreateContext(CustomerType.Regular, 200m);

        var result = engine.Resolve(context);

        Assert.NotNull(result);
        Assert.Equal("first-10", result.PolicyId);
        Assert.Equal(20m, result.Amount);
    }

    [Fact]
    public void InvoiceCalculator_WhenCompetingDiscounts_ExposesOnlyWinningDiscountOnInvoice()
    {
        var calculator = TestFactory.CreateCalculator(
        [
            new PremiumCustomerDiscount(),
            new PercentageTestDiscount("test-20-percent", "Test 20% off", 0.20m)
        ]);

        var cart = new Cart(
            new Customer("Premium User", CustomerType.Premium),
            [new LineItem("Widget", 100m, 2)]);

        var invoice = calculator.Calculate(cart, FixedDate);

        Assert.Equal(200m, invoice.Subtotal);
        Assert.NotNull(invoice.Discount);
        Assert.Equal("test-20-percent", invoice.Discount.PolicyId);
        Assert.Equal(40m, invoice.Discount.Amount);
        // Tax on discounted subtotal: (200 - 40) * 8% = 12.80
        Assert.Equal(12.80m, invoice.Tax.Amount);
        Assert.Equal(50m, invoice.Shipping.Amount);
        Assert.Equal(222.80m, invoice.GrandTotal);
    }
}
