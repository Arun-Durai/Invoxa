namespace Invoxa.Domain;

public sealed class Customer
{
    public string Name { get; }
    public CustomerType Type { get; }

    public Customer(string name, CustomerType type)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Name cannot be null, empty, or whitespace.",
                nameof(name));
        }
        
        Name = name;
        Type = type;
    }
}
