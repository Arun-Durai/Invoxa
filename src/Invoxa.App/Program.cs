using Invoxa;
using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Output;
using Invoxa.Pricing;
using Invoxa.Tax;

// Hardcoded sample carts for Phase 1.
// Later: replace with config-file loading (e.g. appsettings.json) without changing calculation logic.
var sampleCarts = new[]
{
    new Cart(
        new Customer("Jack Sparrow", CustomerType.Regular),
        [
            new LineItem("Wireless Mouse", 799m, 1),
            new LineItem("USB-C Hub", 1499m, 1),
            new LineItem("Notebook (A5)", 120m, 3)
        ]),
    new Cart(
        new Customer("Elizabeth Swann", CustomerType.Premium),
        [
            new LineItem("Mechanical Keyboard", 4599m, 1),
            new LineItem("Desk Lamp", 899m, 2),
            new LineItem("HDMI Cable", 349m, 2)
        ])
};

var discountPolicies = new IDiscountPolicy[] { new PremiumCustomerDiscount() };
var discountEngine = new DiscountEngine(discountPolicies);
var taxPolicy = new FlatRateTaxPolicy(0.08m);
var calculator = new InvoiceCalculator(discountEngine, taxPolicy);
var billingService = new BillingService(calculator);
var formatter = new ConsoleTextFormatter();

Console.WriteLine();
Console.WriteLine("  Invoxa — Phase 1 billing demo");
Console.WriteLine("  ─────────────────────────────");
Console.WriteLine();

for (var i = 0; i < sampleCarts.Length; i++)
{
    if (i > 0)
    {
        Console.WriteLine();
        Console.WriteLine(new string('─', 64));
        Console.WriteLine();
    }

    var invoice = billingService.GenerateInvoice(sampleCarts[i]);
    Console.WriteLine(formatter.Format(invoice));
}

Console.WriteLine();
