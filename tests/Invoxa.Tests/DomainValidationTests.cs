using Invoxa.Domain;

namespace Invoxa.Tests;

public class DomainValidationTests
{
    [Fact]
    public void LineItem_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new LineItem("", 100m, 1));
    }

    [Fact]
    public void LineItem_NegativeUnitPrice_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new LineItem("Widget", -10m, 1));
    }

    [Fact]
    public void LineItem_ZeroQuantity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new LineItem("Widget", 100m, 0));
    }

    [Fact]
    public void Customer_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new Customer(" ", CustomerType.Regular));
    }

    [Fact]
    public void Cart_NullLineItems_ThrowsArgumentNullException()
    {
        var customer = new Customer("John", CustomerType.Regular);

        Assert.Throws<ArgumentNullException>(
            () => new Cart(customer, null!));
    }

    [Fact]
    public void Cart_EmptyLineItems_ThrowsArgumentException()
    {
        var customer = new Customer("John", CustomerType.Regular);

        Assert.Throws<ArgumentException>(
            () => new Cart(customer, []));
    }
}