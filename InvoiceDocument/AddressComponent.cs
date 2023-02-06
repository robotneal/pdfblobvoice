using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace InvoiceToPdf.Invoices;

public class AddressComponent : IComponent
{
    private string Title { get; }
    private Address Address { get; }

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

            column.Item().Text(Address.CompanyName);
            column.Item().Text(Address.Street);
            column.Item().Text($"{Address.City}, {Address.State}");
            column.Item().Text(Address.Email);
            column.Item().Text(Address.Phone);
        });
    }
}
