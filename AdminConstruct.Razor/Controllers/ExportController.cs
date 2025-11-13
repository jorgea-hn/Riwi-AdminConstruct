using AdminConstruct.Razor.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;


namespace AdminConstruct.Razor.Controllers
{
    public class ExportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -----------------------------
        // PRODUCTOS
        // -----------------------------

        [HttpGet]
        public IActionResult ExportProductsToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Productos");

            worksheet.Cells[1, 1].Value = "Nombre";
            worksheet.Cells[1, 2].Value = "Precio";
            worksheet.Cells[1, 3].Value = "Stock";
            worksheet.Cells[1, 4].Value = "Descripción";

            var products = _context.Products.ToList();
            for (int i = 0; i < products.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = products[i].Name;
                worksheet.Cells[i + 2, 2].Value = products[i].Price;
                worksheet.Cells[i + 2, 3].Value = products[i].StockQuantity;
                worksheet.Cells[i + 2, 4].Value = products[i].Description;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            var fileName = $"Productos_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public IActionResult ExportProductsToPDF()
        {
            QuestPDF.Settings.License = LicenseType.Community; 
            var products = _context.Products.ToList();
            var fileName = $"Productos_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/recibos");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            var fullPath = Path.Combine(filePath, fileName);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Header().Text("Listado de Productos").FontSize(20).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); // Nombre
                            columns.RelativeColumn(); // Email / Precio
                            columns.RelativeColumn(); // Documento / Stock
                            columns.RelativeColumn(); // Teléfono / Descripción
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Nombre").Bold();
                            header.Cell().Text("Precio").Bold();
                            header.Cell().Text("Stock").Bold();
                            header.Cell().Text("Descripción").Bold();
                        });

                        foreach (var p in products)
                        {
                            table.Cell().Text(p.Name);
                            table.Cell().Text(p.Price.ToString("C"));
                            table.Cell().Text(p.StockQuantity.ToString());
                            table.Cell().Text(p.Description);
                        }
                    });

                    page.Footer().AlignCenter().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });

            document.GeneratePdf(fullPath);
            var bytes = System.IO.File.ReadAllBytes(fullPath);
            return File(bytes, "application/pdf", fileName);
        }

        // -----------------------------
        // CLIENTES
        // -----------------------------

        [HttpGet]
        public IActionResult ExportClientsToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Clientes");

            worksheet.Cells[1, 1].Value = "Nombre";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Documento";
            worksheet.Cells[1, 4].Value = "Teléfono";

            var clients = _context.Customers.ToList();
            for (int i = 0; i < clients.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = clients[i].Name;
                worksheet.Cells[i + 2, 2].Value = clients[i].Email;
                worksheet.Cells[i + 2, 3].Value = clients[i].Document;
                worksheet.Cells[i + 2, 4].Value = clients[i].Phone;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            var fileName = $"Clientes_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public IActionResult ExportClientsToPDF()
        {
            QuestPDF.Settings.License = LicenseType.Community; 
            var clients = _context.Customers.ToList();
            var fileName = $"Clientes_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/recibos");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            var fullPath = Path.Combine(filePath, fileName);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Header().Text("Listado de Clientes").FontSize(20).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); // Nombre
                            columns.RelativeColumn(); // Email / Precio
                            columns.RelativeColumn(); // Documento / Stock
                            columns.RelativeColumn(); // Teléfono / Descripción
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Nombre").Bold();
                            header.Cell().Text("Email").Bold();
                            header.Cell().Text("Documento").Bold();
                            header.Cell().Text("Teléfono").Bold();
                        });

                        foreach (var c in clients)
                        {
                            table.Cell().Text(c.Name);
                            table.Cell().Text(c.Email);
                            table.Cell().Text(c.Document);
                            table.Cell().Text(c.Phone);
                        }
                    });

                    page.Footer().AlignCenter().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });

            document.GeneratePdf(fullPath);
            var bytes = System.IO.File.ReadAllBytes(fullPath);
            return File(bytes, "application/pdf", fileName);
        }
        
        // -----------------------------
        // VENTAS
        // -----------------------------

        [HttpGet]
        public IActionResult ExportSalesToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Ventas");

            worksheet.Cells[1, 1].Value = "Cliente";
            worksheet.Cells[1, 2].Value = "Fecha";
            worksheet.Cells[1, 3].Value = "Producto";
            worksheet.Cells[1, 4].Value = "Cantidad";
            worksheet.Cells[1, 5].Value = "Precio Unitario";
            worksheet.Cells[1, 6].Value = "Subtotal";

            var sales = _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Details)
                    .ThenInclude(d => d.Product)
                .ToList();

            int row = 2;
            foreach (var sale in sales)
            {
                foreach (var detail in sale.Details)
                {
                    worksheet.Cells[row, 1].Value = sale.Customer.Name;
                    worksheet.Cells[row, 2].Value = sale.Date.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 3].Value = detail.Product.Name;
                    worksheet.Cells[row, 4].Value = detail.Quantity;
                    worksheet.Cells[row, 5].Value = detail.UnitPrice;
                    worksheet.Cells[row, 6].Value = detail.Quantity * detail.UnitPrice;
                    row++;
                }
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            var fileName = $"Ventas_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

      [HttpGet]
public IActionResult ExportSalesToPDF()
{
    QuestPDF.Settings.License = LicenseType.Community;

    var sales = _context.Sales
        .Include(s => s.Customer)
        .Include(s => s.Details)
            .ThenInclude(d => d.Product)
        .ToList();

    var fileName = $"Ventas_{DateTime.Now:yyyyMMddHHmmss}.pdf";
    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/recibos");
    if (!Directory.Exists(folderPath))
        Directory.CreateDirectory(folderPath);

    var fullPath = Path.Combine(folderPath, fileName);

    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.Header().Text("Listado de Ventas").FontSize(20).Bold().AlignCenter();

            page.Content().Table(table =>
            {
                // Definición de columnas
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Cliente / Producto
                    columns.RelativeColumn(2); // Fecha
                    columns.RelativeColumn(1); // Cantidad
                    columns.RelativeColumn(2); // Precio Unitario
                    columns.RelativeColumn(2); // Subtotal
                });

                // Encabezado de la tabla
                table.Header(header =>
                {
                    header.Cell().Text("Cliente").Bold();
                    header.Cell().Text("Fecha").Bold();
                    header.Cell().Text("Cantidad").Bold();
                    header.Cell().Text("Precio Unitario").Bold();
                    header.Cell().Text("Subtotal").Bold();
                });

                // Filas de la tabla
                foreach (var sale in sales)
                {
                    foreach (var detail in sale.Details)
                    {
                        table.Cell().Text(sale.Customer.Name);
                        table.Cell().Text(sale.Date.ToString("dd/MM/yyyy HH:mm"));
                        table.Cell().Text(detail.Quantity.ToString());
                        table.Cell().Text(detail.UnitPrice.ToString("C"));
                        table.Cell().Text((detail.Quantity * detail.UnitPrice).ToString("C"));
                    }

                    // Totales de la venta (una fila separada)
                    var subtotal = sale.Details.Sum(d => d.Quantity * d.UnitPrice);
                    var iva = subtotal * 0.12m;
                    var total = subtotal + iva;

                    table.Cell().ColumnSpan(4).AlignRight().Text("Subtotal:").Bold();
                    table.Cell().Text($"{subtotal:C}");

                    table.Cell().ColumnSpan(4).AlignRight().Text("IVA (12%):").Bold();
                    table.Cell().Text($"{iva:C}");

                    table.Cell().ColumnSpan(4).AlignRight().Text("Total:").Bold();
                    table.Cell().Text($"{total:C}");
                }
            });

            page.Footer().AlignCenter().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}");
        });
    });

    document.GeneratePdf(fullPath);
    var bytes = System.IO.File.ReadAllBytes(fullPath);
    return File(bytes, "application/pdf", fileName);
}




    }
}
