using System.Text.Json;
using Invoxa.Domain;
using Invoxa.Output;

namespace Invoxa.Tests;

public class JsonInvoiceFormatterTests
{
    [Fact]
    public void Format_ProducesExpectedJsonStructure()
    {
        var invoice = new Invoice(
            new Customer("Jane Doe", CustomerType.PremiumPlus),
            [new InvoiceLine("Test Item", 100m, 2, 200m)],
            subtotal: 200m,
            discount: new AppliedDiscount("premium-plus-customer", "Premium+ member (15% off)", 30m),
            tax: new TaxLine("Sales tax", 0.08m, 13.60m),
            shipping: new ShippingLine("Free shipping", 0m),
            grandTotal: 183.60m,
            generatedAt: new DateTime(2026, 7, 3, 10, 0, 0));

        var json = new JsonInvoiceFormatter().Format(invoice);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.Equal("Jane Doe", root.GetProperty("customer").GetProperty("name").GetString());
        Assert.Equal("PremiumPlus", root.GetProperty("customer").GetProperty("type").GetString());
        Assert.Equal(200m, root.GetProperty("subtotal").GetDecimal());
        Assert.Equal("premium-plus-customer", root.GetProperty("discount").GetProperty("policyId").GetString());
        Assert.Equal(13.60m, root.GetProperty("tax").GetProperty("amount").GetDecimal());
        Assert.Equal(0m, root.GetProperty("shipping").GetProperty("amount").GetDecimal());
        Assert.Equal(183.60m, root.GetProperty("grandTotal").GetDecimal());
        Assert.Equal(1, root.GetProperty("lineItems").GetArrayLength());
    }
}
