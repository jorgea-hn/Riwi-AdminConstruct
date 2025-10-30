using AdminConstruct.Ryzor.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AdminConstruct.Ryzor.Services;

public class PdfReceiptService
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public PdfReceiptService(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<string> GenerateReceiptAsync(Guid saleId)
    {
        var sale = await _db.Sales
            .Include(s => s.Details)
            .ThenInclude(d => d.Product)
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == saleId);
        if (sale == null) throw new InvalidOperationException("Venta no encontrada");

        var receiptsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "recibos");
        Directory.CreateDirectory(receiptsDir);
        var filePath = Path.Combine(receiptsDir, $"recibo_{sale.Id}.pdf");

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Text("Recibo de venta").SemiBold().FontSize(18).AlignCenter();
                page.Content().Column(col =>
                {
                    col.Item().Text($"Fecha: {sale.Date:yyyy-MM-dd}");
                    col.Item().Text($"Cliente: {sale.Customer.Name} ({sale.Customer.Document})");
                    col.Item().Text("");
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(5);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });
                        table.Header(header =>
                        {
                            header.Cell().Element(CellHeader).Text("Producto");
                            header.Cell().Element(CellHeader).Text("Cantidad");
                            header.Cell().Element(CellHeader).Text("Precio");
                            header.Cell().Element(CellHeader).Text("Subtotal");
                        });

                        decimal total = 0m;
                        foreach (var d in sale.Details)
                        {
                            var sub = d.Quantity * d.UnitPrice;
                            total += sub;
                            table.Cell().Element(CellBody).Text(d.Product.Name);
                            table.Cell().Element(CellBody).Text(d.Quantity.ToString());
                            table.Cell().Element(CellBody).Text(d.UnitPrice.ToString("C"));
                            table.Cell().Element(CellBody).Text(sub.ToString("C"));
                        }

                        var iva = total * 0.19m;
                        var totalIva = total + iva;

                        table.Cell().ColumnSpan(3).Element(CellBody).Text("IVA (19%)");
                        table.Cell().Element(CellBody).Text(iva.ToString("C"));
                        table.Cell().ColumnSpan(3).Element(CellBodyBold).Text("Total");
                        table.Cell().Element(CellBodyBold).Text(totalIva.ToString("C"));
                    });
                });
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Firmeza - Sistema Administrativo");
                });
            });
        });

        doc.GeneratePdf(filePath);
        return $"/recibos/recibo_{sale.Id}.pdf";

        IContainer CellHeader(IContainer c) => c.Padding(5).Background(Colors.Grey.Lighten3);
        IContainer CellBody(IContainer c) => c.Padding(5);
        IContainer CellBodyBold(IContainer c) => c.Padding(5).Bold();
    }
}


