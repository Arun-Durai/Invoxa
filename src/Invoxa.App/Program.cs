using Invoxa;
using Invoxa.Discounts;
using Invoxa.Domain;
using Invoxa.Output;
using Invoxa.Pricing;
using Invoxa.Tax;

// Hardcoded sample carts for Phase 2 demo.
// Later: replace with config-file loading (e.g. appsettings.json) without changing calculation logic.
var b2g1ProductId = "PROMO-B2G1";

var sampleScenarios = new (Cart Cart, DateTime BillingDate, string Title)[]
{
    (
        new Cart(
            new Customer("Jack Sparrow", CustomerType.Regular),
            [
                new LineItem("Wireless Mouse", 799m, 1),
                new LineItem("USB-C Hub", 1499m, 1),
                new LineItem("Notebook (A5)", 120m, 3)
            ]),
        new DateTime(2026, 12, 20, 10, 0, 0),
        "Regular customer — December seasonal discount"),
    (
        new Cart(
            new Customer("Elizabeth Swann", CustomerType.Premium),
            [
                new LineItem("Mechanical Keyboard", 4599m, 1),
                new LineItem("Desk Lamp", 899m, 2),
                new LineItem("HDMI Cable", 349m, 2)
            ]),
        new DateTime(2026, 12, 20, 10, 0, 0),
        "Premium customer — December beats Premium 10%"),
    (
        new Cart(
            new Customer("Hector Barbossa", CustomerType.PremiumPlus),
            [
                new LineItem("Gaming Headset", 2999m, 1),
                new LineItem("Mouse Pad", 499m, 2)
            ]),
        new DateTime(2026, 7, 15, 10, 0, 0),
        "Premium+ customer — 15% off and free shipping"),
    (
        new Cart(
            new Customer("Will Turner", CustomerType.Regular),
            [
                new LineItem("Coffee Mug", 300m, 3, b2g1ProductId),
                new LineItem("Screen Cleaner", 150m, 1)
            ]),
        new DateTime(2026, 7, 15, 10, 0, 0),
        "Buy 2 Get 1 Free on qualifying item")
};

var discountPolicies = new IDiscountPolicy[]
{
    new PremiumCustomerDiscount(),
    new PremiumPlusCustomerDiscount(),
    new DecemberSeasonalDiscount(),
    new Buy2Get1FreeDiscount([b2g1ProductId])
};

var discountEngine = new DiscountEngine(discountPolicies);
var taxPolicy = new FlatRateTaxPolicy(0.08m);
var calculator = new InvoiceCalculator(discountEngine, taxPolicy);
var billingService = new BillingService(calculator);
var consoleFormatter = new ConsoleTextFormatter();
var jsonFormatter = new JsonInvoiceFormatter();

Console.WriteLine();
Console.WriteLine("  Invoxa — Phase 2 billing demo");
Console.WriteLine("  ─────────────────────────────");
Console.WriteLine();

for (var i = 0; i < sampleScenarios.Length; i++)
{
    if (i > 0)
    {
        Console.WriteLine();
        Console.WriteLine(new string('─', 64));
        Console.WriteLine();
    }

    var (cart, billingDate, title) = sampleScenarios[i];
    var invoice = billingService.GenerateInvoice(cart, billingDate);

    Console.WriteLine($"  {title}");
    Console.WriteLine();
    Console.WriteLine(consoleFormatter.Format(invoice));

    if (i == 0)
    {
        Console.WriteLine();
        Console.WriteLine("  JSON export (accounting API):");
        Console.WriteLine();
        Console.WriteLine(jsonFormatter.Format(invoice));
    }
}

Console.WriteLine();
