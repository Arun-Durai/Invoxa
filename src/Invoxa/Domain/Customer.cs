namespace Invoxa.Domain;

public sealed class Customer
{
    public string Name { get; }
    public CustomerType Type { get; }

    public Customer(string name, CustomerType type)
    {
        Name = name;
        Type = type;
    }
}
