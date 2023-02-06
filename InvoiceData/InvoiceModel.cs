using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceToPdf.Invoices;

public class InvoiceModel
{
    public string InvoiceNumber { get; set; } = "";
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }

    public Address SellerAddress { get; set; } = new();
    public Address CustomerAddress { get; set; } = new();

    public IEnumerable<OrderItem> Items { get; set; } = Enumerable.Empty<OrderItem>();
    public string Comments { get; set; } = "";
}

public class OrderItem
{
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public decimal VAT { get; set; }
    public int Quantity { get; set; }
}

public class Address
{
    public string CompanyName { get; set; } = "";
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public object Email { get; set; } = "";
    public string Phone { get; set; } = "";
}
