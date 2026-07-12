namespace Invoxa.Numbering;

public interface IInvoiceNumberGenerator
{
    string Next(DateTime asOf);
}
