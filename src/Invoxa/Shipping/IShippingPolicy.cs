using Invoxa.Domain;
using Invoxa.Pricing;

namespace Invoxa.Shipping;

public interface IShippingPolicy
{
    ShippingLine Calculate(Customer customer, PricingContext context);
}
