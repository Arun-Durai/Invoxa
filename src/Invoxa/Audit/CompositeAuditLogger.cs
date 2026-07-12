using Invoxa.Audit;
using Invoxa.Domain;

namespace Invoxa.Audit;

public sealed class CompositeAuditLogger : IAuditLogger
{
    private readonly IReadOnlyList<IAuditLogger> _loggers;

    public CompositeAuditLogger(params IAuditLogger[] loggers)
    {
        _loggers = loggers;
    }

    public void LogInvoiceGenerated(Invoice invoice)
    {
        foreach (var logger in _loggers)
            logger.LogInvoiceGenerated(invoice);
    }
}
