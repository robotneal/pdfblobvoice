using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace InvoiceToPdf.Invoices;

public class AddressComponent : IComponent
{
    private string Title { get; init; }
    private Address Address { get; init; }

    public AddressComponent(string title, Address address)
    {
        Title = title;
        Address = address;
    }

    public void Compose(IContainer container)
    {
        container.ShowEntire().Column(column =>
        {
            column.Spacing(2);

            column.Item().Text(Title).SemiBold();
            column.Item().PaddingBottom(5).LineHorizontal(1);

            column.Item().Text(Address.Name);
            column.Item().Text(Address.Street);
            column.Item().Text($"{Address.City}, {Address.County}");
        });
    }
}

public class ComapnyComponent : IComponent
{
    private string Title { get; init; }
    private Company Company { get; init; }

    public ComapnyComponent(string title, Company company)
    {
        Title = title;
        Company = company;
    }

    public void Compose(IContainer container)
    {
        container.ShowEntire().Column(column =>
        {
            column.Spacing(2);

            column.Item().Text(Title).SemiBold();
            column.Item().PaddingBottom(5).LineHorizontal(1);

            column.Item().Text(Company.Name);
            column.Item().Text(Company.Address.Street);
            column.Item().Text($"{Company.Address.City}, {Company.Address.County}");
        });
    }
}
