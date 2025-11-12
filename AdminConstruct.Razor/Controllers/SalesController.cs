using AdminConstruct.Razor.Data;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.Razor.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }

        // GET: Sales
        public async Task<IActionResult> Sales()
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Details)
                .ThenInclude(d => d.Product)
                .OrderByDescending(s => s.Date)
                .Select(s => new AdminConstruct.Ryzor.ViewModels.SaleViewModel
                {
                    Id = s.Id,
                    CustomerName = s.Customer.Name, // ya existe
                    Date = s.Date,
                    Details = s.Details.Select(d => new AdminConstruct.Ryzor.ViewModels.SaleDetailViewModel
                    {
                        Id = d.Id,
                        ProductName = d.Product.Name,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice
                    }).ToList()
                })
                .ToListAsync();

            return View(sales);
        }


        // GET: Sales/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Details)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return NotFound();

            return View(sale);
        }

        // GET: Sales/ExportExcel
        public async Task<IActionResult> ExportExcel()
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

            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Details)
                    .ThenInclude(d => d.Product)
                .ToListAsync();

            int row = 2;
            foreach (var s in sales)
            {
                foreach (var d in s.Details)
                {
                    worksheet.Cells[row, 1].Value = s.Customer.Name;
                    worksheet.Cells[row, 2].Value = s.Date.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 3].Value = d.Product.Name;
                    worksheet.Cells[row, 4].Value = d.Quantity;
                    worksheet.Cells[row, 5].Value = d.UnitPrice;
                    worksheet.Cells[row, 6].Value = d.Quantity * d.UnitPrice;
                    row++;
                }
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            var fileName = $"Ventas_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


    }
}
