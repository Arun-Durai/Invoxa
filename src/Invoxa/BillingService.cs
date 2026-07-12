using Invoxa.Audit;
using Invoxa.Domain;
using Invoxa.Numbering;
using Invoxa.Pricing;

namespace Invoxa;

public sealed class BillingService
{
    private readonly InvoiceCalculator _calculator;
    private readonly IInvoiceNumberGenerator _numberGenerator;
    private readonly IAuditLogger _auditLogger;

    public BillingService(
        InvoiceCalculator calculator,
        IInvoiceNumberGenerator numberGenerator,
        IAuditLogger auditLogger)
    {
        _calculator = calculator;
        _numberGenerator = numberGenerator;
        _auditLogger = auditLogger;
    }

    public Invoice GenerateInvoice(Cart cart, DateTime? asOf = null)
    {
        var billingDate = asOf ?? DateTime.Now;
        var invoice = _calculator.Calculate(cart, billingDate);
        invoice = invoice.WithInvoiceNumber(_numberGenerator.Next(billingDate));
        _auditLogger.LogInvoiceGenerated(invoice);
        return invoice;
    }
}
