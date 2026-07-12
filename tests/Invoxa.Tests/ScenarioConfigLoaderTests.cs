using Invoxa.App.Configuration;
using Invoxa.Domain;

namespace Invoxa.Tests;

public class ScenarioConfigLoaderTests
{
    [Fact]
    public void ToScenarios_MapsConfigToDomainCarts()
    {
        var settings = new BillingAppSettings
        {
            Scenarios =
            [
                new ScenarioSettings
                {
                    Title = "Demo",
                    BillingDate = new DateTime(2026, 12, 1),
                    Customer = new CustomerSettings { Name = "Config User", Type = "Premium" },
                    LineItems =
                    [
                        new LineItemSettings
                        {
                            Name = "Mouse",
                            UnitPrice = 500m,
                            Quantity = 2,
                            Category = "Electronics"
                        }
                    ]
                }
            ]
        };

        var scenarios = new ScenarioConfigLoader().ToScenarios(settings);

        Assert.Single(scenarios);
        Assert.Equal("Demo", scenarios[0].Title);
        Assert.Equal("Config User", scenarios[0].Cart.Customer.Name);
        Assert.Equal(CustomerType.Premium, scenarios[0].Cart.Customer.Type);
        Assert.Equal(ProductCategory.Electronics, scenarios[0].Cart.LineItems[0].Category);
        Assert.Equal(1000m, scenarios[0].Cart.LineItems[0].LineTotal);
    }

    [Fact]
    public void ParseCustomerType_Unknown_Throws()
    {
        Assert.Throws<ArgumentException>(() => ScenarioConfigLoader.ParseCustomerType("Gold"));
    }
}
