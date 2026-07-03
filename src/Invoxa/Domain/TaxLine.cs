namespace Invoxa.Domain;

public sealed class TaxLine
{
    public string Label { get; }
    public decimal Rate { get; }
    public decimal Amount { get; }

    public TaxLine(string label, decimal rate, decimal amount)
    {
        Label = label;
        Rate = rate;
        Amount = amount;
    }
}
