using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa;

public sealed class BillingService
{
    private readonly InvoiceCalculator _calculator;

    public BillingService(InvoiceCalculator calculator)
    {
        _calculator = calculator;
    }

    public Invoice GenerateInvoice(Cart cart, DateTime? asOf = null) =>
        _calculator.Calculate(cart, asOf ?? DateTime.Now);
}
