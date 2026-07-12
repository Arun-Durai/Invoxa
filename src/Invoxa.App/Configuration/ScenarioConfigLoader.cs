using Invoxa.Domain;
using Microsoft.Extensions.Configuration;

namespace Invoxa.App.Configuration;

public sealed class ScenarioConfigLoader
{
    public BillingAppSettings Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Configuration file not found: {path}", path);

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(path, optional: false, reloadOnChange: false)
            .Build();

        var settings = new BillingAppSettings();
        configuration.Bind(settings);

        if (settings.Scenarios.Count == 0)
            throw new InvalidOperationException("Configuration must define at least one scenario.");

        return settings;
    }

    public IReadOnlyList<(Cart Cart, DateTime BillingDate, string Title)> ToScenarios(BillingAppSettings settings)
    {
        return settings.Scenarios
            .Select(scenario =>
            {
                var customer = new Customer(scenario.Customer.Name, ParseCustomerType(scenario.Customer.Type));
                var items = scenario.LineItems
                    .Select(item => new LineItem(
                        item.Name,
                        item.UnitPrice,
                        item.Quantity,
                        item.ProductId,
                        ParseCategory(item.Category)))
                    .ToList();

                return (new Cart(customer, items), scenario.BillingDate, scenario.Title);
            })
            .ToList();
    }

    public static CustomerType ParseCustomerType(string value) =>
        Enum.TryParse<CustomerType>(value, ignoreCase: true, out var type)
            ? type
            : throw new ArgumentException($"Unknown customer type: '{value}'.");

    public static ProductCategory ParseCategory(string value) =>
        Enum.TryParse<ProductCategory>(value, ignoreCase: true, out var category)
            ? category
            : throw new ArgumentException($"Unknown product category: '{value}'.");
}
