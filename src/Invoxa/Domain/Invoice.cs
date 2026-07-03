namespace Invoxa.Domain;

public sealed class Invoice
{
    public Customer Customer { get; }
    public IReadOnlyList<InvoiceLine> Lines { get; }
    public decimal Subtotal { get; }
    public AppliedDiscount? Discount { get; }
    public TaxLine Tax { get; }
    public decimal GrandTotal { get; }
    public DateTime GeneratedAt { get; }

    public Invoice(
        Customer customer,
        IReadOnlyList<InvoiceLine> lines,
        decimal subtotal,
        AppliedDiscount? discount,
        TaxLine tax,
        decimal grandTotal,
        DateTime generatedAt)
    {
        Customer = customer;
        Lines = lines;
        Subtotal = subtotal;
        Discount = discount;
        Tax = tax;
        GrandTotal = grandTotal;
        GeneratedAt = generatedAt;
    }
}
