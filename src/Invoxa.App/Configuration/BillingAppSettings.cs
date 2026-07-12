namespace Invoxa.App.Configuration;

public sealed class BillingAppSettings
{
    public List<ScenarioSettings> Scenarios { get; set; } = [];
    public List<string> PromoProductIds { get; set; } = [];
}

public sealed class ScenarioSettings
{
    public string Title { get; set; } = "";
    public DateTime BillingDate { get; set; }
    public CustomerSettings Customer { get; set; } = new();
    public List<LineItemSettings> LineItems { get; set; } = [];
}

public sealed class CustomerSettings
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "Regular";
}

public sealed class LineItemSettings
{
    public string Name { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? ProductId { get; set; }
    public string Category { get; set; } = "General";
}
