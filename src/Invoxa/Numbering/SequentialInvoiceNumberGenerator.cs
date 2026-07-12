namespace Invoxa.Numbering;

/// <summary>
/// In-memory sequential numbers for the current process. Resets when the app restarts.
/// </summary>
public sealed class SequentialInvoiceNumberGenerator : IInvoiceNumberGenerator
{
    private int _sequence;

    public string Next(DateTime asOf)
    {
        var next = Interlocked.Increment(ref _sequence);
        return $"INV-{asOf.Year}-{next:D5}";
    }
}
