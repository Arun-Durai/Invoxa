using Invoxa.Domain;

namespace Invoxa.Audit;

public sealed class ConsoleAuditLogger : IAuditLogger
{
    public void LogInvoiceGenerated(Invoice invoice)
    {
        Console.WriteLine(
            $"[AUDIT] {invoice.InvoiceNumber} | {invoice.Customer.Name} | {invoice.GeneratedAt:O} | {invoice.GrandTotal:0.00}");
    }
}
