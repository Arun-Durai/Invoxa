using System.Globalization;
using Invoxa.Domain;

namespace Invoxa.Audit;

public sealed class FileAuditLogger : IAuditLogger
{
    private readonly string _filePath;
    private readonly object _sync = new();

    public FileAuditLogger(string filePath)
    {
        _filePath = filePath;
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
    }

    public void LogInvoiceGenerated(Invoice invoice)
    {
        var line = string.Format(
            CultureInfo.InvariantCulture,
            "{0:O}\t{1}\t{2}\t{3:0.00}{4}",
            invoice.GeneratedAt,
            invoice.InvoiceNumber,
            invoice.Customer.Name,
            invoice.GrandTotal,
            Environment.NewLine);

        lock (_sync)
        {
            File.AppendAllText(_filePath, line);
        }
    }
}
