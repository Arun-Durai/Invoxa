using System.Globalization;
using System.Text;
using Invoxa.Domain;

namespace Invoxa.Output;

public sealed class ConsoleTextFormatter : IInvoiceFormatter
{
    private static readonly CultureInfo InrCulture = CreateInrCulture();

    public string Format(Invoice invoice)
    {
        var sb = new StringBuilder();
        var width = 62;

        AppendBorder(sb, width, '╔', '═', '╗');
        AppendCentered(sb, width, "INVOXA");
        AppendCentered(sb, width, "Store Billing & Invoice");
        AppendBorder(sb, width, '╠', '═', '╣');

        AppendRow(sb, width, "Customer", invoice.Customer.Name);
        AppendRow(sb, width, "Type", FormatCustomerType(invoice.Customer.Type));
        AppendRow(sb, width, "Date", invoice.GeneratedAt.ToString("dd MMM yyyy, hh:mm tt"));

        AppendBorder(sb, width, '╠', '═', '╣');
        AppendLine(sb, width, "  ITEMS");
        AppendBorder(sb, width, '╟', '─', '╢');

        AppendLine(sb, width, PadColumns("Item", "Qty", "Unit Price", "Total"));
        AppendBorder(sb, width, '╟', '─', '╢');

        foreach (var line in invoice.Lines)
        {
            var itemColumn = Truncate(line.Name, 28);
            var row = PadColumns(
                itemColumn,
                line.Quantity.ToString(CultureInfo.InvariantCulture),
                FormatCurrency(line.UnitPrice),
                FormatCurrency(line.LineTotal));
            AppendLine(sb, width, row);
        }

        AppendBorder(sb, width, '╠', '═', '╣');

        AppendAmountRow(sb, width, "Subtotal", invoice.Subtotal);

        if (invoice.Discount is { } discount)
        {
            AppendAmountRow(sb, width, discount.Label, -discount.Amount);
        }
        else
        {
            AppendRow(sb, width, "Discount", "—");
        }

        var taxLabel = $"{invoice.Tax.Label} ({invoice.Tax.Rate:P0})";
        AppendAmountRow(sb, width, taxLabel, invoice.Tax.Amount);
        AppendAmountRow(sb, width, invoice.Shipping.Label, invoice.Shipping.Amount);

        AppendBorder(sb, width, '╠', '═', '╣');
        AppendAmountRow(sb, width, "GRAND TOTAL", invoice.GrandTotal, emphasize: true);
        AppendBorder(sb, width, '╚', '═', '╝');

        return sb.ToString();
    }

    private static string FormatCustomerType(CustomerType type) =>
        type switch
        {
            CustomerType.Regular => "Regular",
            CustomerType.Premium => "Premium",
            CustomerType.PremiumPlus => "Premium+",
            _ => type.ToString()
        };

    private static string FormatCurrency(decimal amount) =>
        amount.ToString("C", InrCulture);

    private static string PadColumns(string col1, string col2, string col3, string col4)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0,-28} {1,4} {2,12} {3,12}",
            col1,
            col2,
            col3,
            col4);
    }

    private static void AppendBorder(StringBuilder sb, int width, char left, char fill, char right)
    {
        sb.Append(left);
        sb.Append(new string(fill, width - 2));
        sb.Append(right);
        sb.AppendLine();
    }

    private static void AppendLine(StringBuilder sb, int width, string content)
    {
        sb.Append('║');
        sb.Append(content.PadRight(width - 2));
        sb.Append('║');
        sb.AppendLine();
    }

    private static void AppendCentered(StringBuilder sb, int width, string text)
    {
        var padding = Math.Max(0, width - 2 - text.Length);
        var left = padding / 2;
        var line = new string(' ', left) + text + new string(' ', padding - left);
        AppendLine(sb, width, line);
    }

    private static void AppendRow(StringBuilder sb, int width, string label, string value)
    {
        var content = $"  {label,-14} {value}";
        AppendLine(sb, width, content);
    }

    private static void AppendAmountRow(StringBuilder sb, int width, string label, decimal amount, bool emphasize = false)
    {
        var formatted = FormatCurrency(Math.Abs(amount));
        if (amount < 0)
            formatted = $"-{formatted}";

        var content = emphasize
            ? $"  {label,-28} {formatted,22}"
            : $"  {label,-36} {formatted,14}";

        AppendLine(sb, width, content);
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..(maxLength - 1)] + "…";

    private static CultureInfo CreateInrCulture()
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.CurrencySymbol = "₹";
        culture.NumberFormat.CurrencyDecimalDigits = 2;
        return culture;
    }
}
