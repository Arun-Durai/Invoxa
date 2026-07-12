using Invoxa.Domain;
using Invoxa.Pricing;
using Invoxa.Tax;

namespace Invoxa.Tests;

public class CategoryTaxPolicyTests
{
    private static readonly DateTime FixedDate = new(2026, 7, 3, 10, 0, 0);

    [Fact]
    public void Calculate_MixedCategories_AppliesDifferentRatesOnDiscountedShares()
    {
        // Electronics 100 @ 8%, Groceries 100 @ 2%, no discount → tax = 8 + 2 = 10
        var policy = new CategoryTaxPolicy();
        var items = new List<LineItem>
        {
            new("Laptop", 100m, 1, category: ProductCategory.Electronics),
            new("Rice", 100m, 1, category: ProductCategory.Groceries)
        };
        var context = new PricingContext(
            new Customer("Test", CustomerType.Regular),
            items,
            200m,
            FixedDate);

        var tax = policy.Calculate(200m, context);

        Assert.Equal(10m, tax.Amount);
        Assert.Equal("Sales tax (by category)", tax.Label);
    }

    [Fact]
    public void InvoiceCalculator_WithCategoryTax_UsesCategoryRates()
    {
        var calculator = TestFactory.CreateCalculator(
            taxPolicy: new CategoryTaxPolicy());

        var cart = new Cart(
            new Customer("Test", CustomerType.Regular),
            [
                new LineItem("Phone", 1000m, 1, category: ProductCategory.Electronics),
                new LineItem("Milk", 100m, 1, category: ProductCategory.Groceries)
            ]);

        var invoice = calculator.Calculate(cart, FixedDate);

        // Taxable 1100; electronics share 1000 → 80; groceries 100 → 2; total tax 82 + shipping 50
        Assert.Equal(1100m, invoice.Subtotal);
        Assert.Equal(82m, invoice.Tax.Amount);
        Assert.Equal(1232m, invoice.GrandTotal);
    }
}
