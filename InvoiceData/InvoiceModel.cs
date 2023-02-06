namespace InvoiceToPdf.Invoices;

public record InvoiceModel
{
    public required string InvoiceNumber { get; init; }
    public required string BookingReference { get; init; }
    public required DateTime IssueDate { get; init; }
    public required DateTime DueDate { get; init; }

    public required Company Company { get; init; }
    public required Address BillingAddress { get; init; }

    public IEnumerable<OrderItem> Items { get; init; } = Enumerable.Empty<OrderItem>();
    public string Notes { get; init; } = "";
}

public record OrderItem(string Name, decimal Price, decimal VAT, int Quantity);

public record Company(string Name, Address Address, string VATNumber, string Number);

public record Address(string Name, string Street, string City, string County, string PostCode);

public static class OrderItemExtensions
{
    public static decimal CalculateTotal(this OrderItem item) => (item.Price * (1m + (item.VAT * 0.01m))) * item.Quantity;
}