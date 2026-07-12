using Invoxa;
using Invoxa.App.Configuration;
using Invoxa.Audit;
using Invoxa.Discounts;
using Invoxa.Numbering;
using Invoxa.Output;
using Invoxa.Pricing;
using Invoxa.Shipping;
using Invoxa.Tax;

var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
var loader = new ScenarioConfigLoader();
var settings = loader.Load(configPath);
var scenarios = loader.ToScenarios(settings);

var promoIds = settings.PromoProductIds.Count > 0
    ? settings.PromoProductIds
    : ["PROMO-B2G1"];

var discountPolicies = new IDiscountPolicy[]
{
    new PremiumCustomerDiscount(),
    new PremiumPlusCustomerDiscount(),
    new DecemberSeasonalDiscount(),
    new Buy2Get1FreeDiscount(promoIds)
};

var discountEngine = new DiscountEngine(discountPolicies);
var taxPolicy = new CategoryTaxPolicy();
var shippingPolicy = new FlatRateShippingPolicy();
var calculator = new InvoiceCalculator(discountEngine, taxPolicy, shippingPolicy);
var billingService = new BillingService(
    calculator,
    new SequentialInvoiceNumberGenerator(),
    new CompositeAuditLogger(
        new ConsoleAuditLogger(),
        new FileAuditLogger(Path.Combine(AppContext.BaseDirectory, "logs", "audit.log"))));

var consoleFormatter = new ConsoleTextFormatter();
var jsonFormatter = new JsonInvoiceFormatter();

Console.WriteLine();
Console.WriteLine("  Invoxa — billing demo (config-driven)");
Console.WriteLine("  ────────────────────────────────────");
Console.WriteLine($"  Config: {configPath}");
Console.WriteLine();

for (var i = 0; i < scenarios.Count; i++)
{
    if (i > 0)
    {
        Console.WriteLine();
        Console.WriteLine(new string('─', 64));
        Console.WriteLine();
    }

    var (cart, billingDate, title) = scenarios[i];
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
