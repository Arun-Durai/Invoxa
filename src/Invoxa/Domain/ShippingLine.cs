namespace Invoxa.Domain;

public sealed class ShippingLine
{
    public string Label { get; }
    public decimal Amount { get; }

    public ShippingLine(string label, decimal amount)
    {
        Label = label;
        Amount = amount;
    }
}
