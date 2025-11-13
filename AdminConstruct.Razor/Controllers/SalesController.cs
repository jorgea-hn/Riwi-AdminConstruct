using AdminConstruct.Razor.Data;
using Microsoft.AspNetCore.Mvc;
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

        // 🔹 LISTAR (GET)
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Details)
                .ThenInclude(d => d.Product)
                .OrderByDescending(s => s.Date)
                .Select(s => new AdminConstruct.Ryzor.ViewModels.SaleViewModel
                {
                    Id = s.Id,
                    CustomerName = s.Customer.Name,
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

            return View("~/Views/Admin/Sales/Sales.cshtml", sales);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Details)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return NotFound();

            return View("~/Views/Admin/Sales/Details.cshtml", sale);
        }
    }
}
