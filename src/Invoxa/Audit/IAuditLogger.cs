using Invoxa.Domain;

namespace Invoxa.Audit;

public interface IAuditLogger
{
    void LogInvoiceGenerated(Invoice invoice);
}
