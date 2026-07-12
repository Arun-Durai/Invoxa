namespace Invoxa.Domain;

public sealed class Cart
{
    public Customer Customer { get; }
    public IReadOnlyList<LineItem> LineItems { get; }

    public Cart(Customer customer, IReadOnlyList<LineItem> lineItems)
    {
        if (lineItems is null)
        {
            throw new ArgumentNullException(nameof(lineItems));
        }

        if (lineItems.Count == 0)
        {
            throw new ArgumentException(
                "Cart must contain at least one line item.",
                nameof(lineItems));
        }

        Customer = customer;
        LineItems = lineItems;
    }
}
