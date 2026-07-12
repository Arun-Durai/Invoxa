namespace Invoxa.Domain;

public sealed class Invoice
{
    public string InvoiceNumber { get; }
    public Customer Customer { get; }
    public IReadOnlyList<InvoiceLine> Lines { get; }
    public decimal Subtotal { get; }
    public AppliedDiscount? Discount { get; }
    public TaxLine Tax { get; }
    public ShippingLine Shipping { get; }
    public decimal GrandTotal { get; }
    public DateTime GeneratedAt { get; }

    public Invoice(
        string invoiceNumber,
        Customer customer,
        IReadOnlyList<InvoiceLine> lines,
        decimal subtotal,
        AppliedDiscount? discount,
        TaxLine tax,
        ShippingLine shipping,
        decimal grandTotal,
        DateTime generatedAt)
    {
        InvoiceNumber = invoiceNumber;
        Customer = customer;
        Lines = lines;
        Subtotal = subtotal;
        Discount = discount;
        Tax = tax;
        Shipping = shipping;
        GrandTotal = grandTotal;
        GeneratedAt = generatedAt;
    }

    public Invoice WithInvoiceNumber(string invoiceNumber) =>
        new(
            invoiceNumber,
            Customer,
            Lines,
            Subtotal,
            Discount,
            Tax,
            Shipping,
            GrandTotal,
            GeneratedAt);
}
