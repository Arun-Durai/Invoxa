namespace Invoxa.Domain;

public sealed class Cart
{
    public Customer Customer { get; }
    public IReadOnlyList<LineItem> LineItems { get; }

    public Cart(Customer customer, IReadOnlyList<LineItem> lineItems)
    {
        Customer = customer;
        LineItems = lineItems;
    }
}
