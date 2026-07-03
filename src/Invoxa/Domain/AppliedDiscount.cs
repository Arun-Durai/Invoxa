namespace Invoxa.Domain;

public sealed class AppliedDiscount
{
    public string PolicyId { get; }
    public string Label { get; }
    public decimal Amount { get; }

    public AppliedDiscount(string policyId, string label, decimal amount)
    {
        PolicyId = policyId;
        Label = label;
        Amount = amount;
    }
}
