namespace Invoxa.Domain;

public sealed class InvoiceLine
{
    public string Name { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }
    public decimal LineTotal { get; }

    public InvoiceLine(string name, decimal unitPrice, int quantity, decimal lineTotal)
    {
        Name = name;
        UnitPrice = unitPrice;
        Quantity = quantity;
        LineTotal = lineTotal;
    }
}
