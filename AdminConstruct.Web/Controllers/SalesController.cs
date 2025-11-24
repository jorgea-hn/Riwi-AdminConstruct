using AdminConstruct.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AdminConstruct.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class SalesController : Controller
{
    private readonly ApplicationDbContext _context;

    public SalesController(ApplicationDbContext context)
    {
        _context = context;
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
    }

    // GET: /Admin/Sales
    public async Task<IActionResult> Index()
    {
        var sales = await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Details)
            .ThenInclude(d => d.Product)
            .OrderByDescending(s => s.Date)
            .Select(s => new AdminConstruct.Web.ViewModels.SaleViewModel
            {
                Id = s.Id,
                CustomerName = s.Customer.Name,
                Date = s.Date,
                Details = s.Details.Select(d => new AdminConstruct.Web.ViewModels.SaleDetailViewModel
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

    // GET: /Admin/Sales/Details/5
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
}
