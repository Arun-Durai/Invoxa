using Invoxa.Domain;
using Invoxa.Output;

namespace Invoxa.Tests;

public class ConsoleTextFormatterTests
{
    [Fact]
    public void Format_IncludesKeyInvoiceSections()
    {
        var invoice = new Invoice(
            "INV-2026-00001",
            new Customer("Jane Doe", CustomerType.Premium),
            [new InvoiceLine("Test Item", 100m, 2, 200m)],
            subtotal: 200m,
            discount: new AppliedDiscount("premium-customer", "Premium member (10% off)", 20m),
            tax: new TaxLine("Sales tax", 0.08m, 14.40m),
            shipping: new ShippingLine("Shipping", 50m),
            grandTotal: 244.40m,
            generatedAt: new DateTime(2026, 7, 3, 10, 0, 0));

        var output = new ConsoleTextFormatter().Format(invoice);

        Assert.Contains("INVOXA", output);
        Assert.Contains("INV-2026-00001", output);
        Assert.Contains("Jane Doe", output);
        Assert.Contains("Test Item", output);
        Assert.Contains("Subtotal", output);
        Assert.Contains("Premium member (10% off)", output);
        Assert.Contains("Shipping", output);
        Assert.Contains("GRAND TOTAL", output);
    }
}
