namespace Invoxa.Domain;

public sealed class LineItem
{
    public string Name { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }
    public decimal LineTotal => Money.Round(UnitPrice * Quantity);

    public LineItem(string name, decimal unitPrice, int quantity)
    {
        Name = name;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
