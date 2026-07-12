using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Pricing;
using Invoxa.Shipping;
using Invoxa.Tax;

namespace Invoxa.Tests;

internal static class TestFactory
{
    public static InvoiceCalculator CreateCalculator(
        IEnumerable<IDiscountPolicy>? policies = null,
        ITaxPolicy? taxPolicy = null,
        IShippingPolicy? shippingPolicy = null) =>
        new(
            new DiscountEngine(policies ?? [new PremiumCustomerDiscount()]),
            taxPolicy ?? new FlatRateTaxPolicy(0.08m),
            shippingPolicy ?? new FlatRateShippingPolicy());
}
