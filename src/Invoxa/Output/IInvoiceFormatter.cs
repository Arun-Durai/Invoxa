using Invoxa.Domain;

namespace Invoxa.Output;

public interface IInvoiceFormatter
{
    string Format(Invoice invoice);
}
