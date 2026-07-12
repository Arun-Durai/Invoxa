using Invoxa;
using Invoxa.Audit;
using Invoxa.Domain;
using Invoxa.Numbering;

namespace Invoxa.Tests;

public class BillingServiceTests
{
    private sealed class CapturingAuditLogger : IAuditLogger
    {
        public List<Invoice> Entries { get; } = [];

        public void LogInvoiceGenerated(Invoice invoice) => Entries.Add(invoice);
    }

    [Fact]
    public void GenerateInvoice_AssignsUniqueNumbers_AndWritesAuditOnce()
    {
        var logger = new CapturingAuditLogger();
        var service = new BillingService(
            TestFactory.CreateCalculator(),
            new SequentialInvoiceNumberGenerator(),
            logger);

        var cart = new Cart(
            new Customer("Audit User", CustomerType.Regular),
            [new LineItem("Item", 100m, 1)]);

        var first = service.GenerateInvoice(cart, new DateTime(2026, 7, 12));
        var second = service.GenerateInvoice(cart, new DateTime(2026, 7, 12));

        Assert.Equal("INV-2026-00001", first.InvoiceNumber);
        Assert.Equal("INV-2026-00002", second.InvoiceNumber);
        Assert.Equal(2, logger.Entries.Count);
        Assert.Equal(first.InvoiceNumber, logger.Entries[0].InvoiceNumber);
        Assert.Equal(second.GrandTotal, logger.Entries[1].GrandTotal);
    }
}
