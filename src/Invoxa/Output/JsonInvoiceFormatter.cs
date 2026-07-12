using System.Text.Json;
using System.Text.Json.Serialization;
using Invoxa.Domain;

namespace Invoxa.Output;

public sealed class JsonInvoiceFormatter : IInvoiceFormatter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string Format(Invoice invoice)
    {
        var payload = new JsonInvoiceDocument
        {
            InvoiceNumber = invoice.InvoiceNumber,
            Customer = new JsonCustomer
            {
                Name = invoice.Customer.Name,
                Type = invoice.Customer.Type.ToString()
            },
            LineItems = invoice.Lines
                .Select(line => new JsonLineItem
                {
                    Name = line.Name,
                    UnitPrice = line.UnitPrice,
                    Quantity = line.Quantity,
                    LineTotal = line.LineTotal
                })
                .ToList(),
            Subtotal = invoice.Subtotal,
            Discount = invoice.Discount is null
                ? null
                : new JsonDiscount
                {
                    PolicyId = invoice.Discount.PolicyId,
                    Label = invoice.Discount.Label,
                    Amount = invoice.Discount.Amount
                },
            Tax = new JsonTax
            {
                Label = invoice.Tax.Label,
                Rate = invoice.Tax.Rate,
                Amount = invoice.Tax.Amount
            },
            Shipping = new JsonShipping
            {
                Label = invoice.Shipping.Label,
                Amount = invoice.Shipping.Amount
            },
            GrandTotal = invoice.GrandTotal,
            GeneratedAt = invoice.GeneratedAt
        };

        return JsonSerializer.Serialize(payload, Options);
    }

    private sealed class JsonInvoiceDocument
    {
        public string InvoiceNumber { get; set; } = "";
        public JsonCustomer Customer { get; set; } = null!;
        public List<JsonLineItem> LineItems { get; set; } = [];
        public decimal Subtotal { get; set; }
        public JsonDiscount? Discount { get; set; }
        public JsonTax Tax { get; set; } = null!;
        public JsonShipping Shipping { get; set; } = null!;
        public decimal GrandTotal { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    private sealed class JsonCustomer
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
    }

    private sealed class JsonLineItem
    {
        public string Name { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }

    private sealed class JsonDiscount
    {
        public string PolicyId { get; set; } = "";
        public string Label { get; set; } = "";
        public decimal Amount { get; set; }
    }

    private sealed class JsonTax
    {
        public string Label { get; set; } = "";
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }

    private sealed class JsonShipping
    {
        public string Label { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
