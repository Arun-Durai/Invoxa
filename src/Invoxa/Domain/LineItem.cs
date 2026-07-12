namespace Invoxa.Domain;

public sealed class LineItem
{
    public string Name { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }
    public string? ProductId { get; }
    public decimal LineTotal => Money.Round(UnitPrice * Quantity);

    public LineItem(string name, decimal unitPrice, int quantity, string? productId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Name cannot be null, empty, or whitespace.",
                nameof(name));
        }

        if (unitPrice < 0)
        {
            throw new ArgumentException(
                "Unit price cannot be negative.",
                nameof(unitPrice));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));
        }

        Name = name;
        UnitPrice = unitPrice;
        Quantity = quantity;
        ProductId = productId;
    }
}
