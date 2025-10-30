using AdminConstruct.Ryzor.Data;
using AdminConstruct.Ryzor.Models;
using AdminConstruct.Ryzor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.Ryzor.Controllers;

[Authorize(Roles = "Admin")]
public class SalesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly PdfReceiptService _pdf;

    public SalesController(ApplicationDbContext db, PdfReceiptService pdf)
    {
        _db = db;
        _pdf = pdf;
    }

    public async Task<IActionResult> Index()
    {
        var sales = await _db.Sales.Include(s => s.Customer).OrderByDescending(s => s.Date).ToListAsync();
        return View(sales);
    }

    public IActionResult Create()
    {
        ViewBag.Customers = new SelectList(_db.Customers.OrderBy(c => c.Name).ToList(), "Id", "Name");
        ViewBag.Products = new SelectList(_db.Products.OrderBy(p => p.Name).ToList(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid customerId, Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0 || unitPrice < 0)
        {
            ModelState.AddModelError(string.Empty, "Cantidad o precio inválido");
            ViewBag.Customers = new SelectList(_db.Customers.OrderBy(c => c.Name).ToList(), "Id", "Name");
            ViewBag.Products = new SelectList(_db.Products.OrderBy(p => p.Name).ToList(), "Id", "Name");
            return View();
        }

        var sale = new Sale { CustomerId = customerId, Date = DateTime.UtcNow };
        _db.Sales.Add(sale);
        var detail = new SaleDetail { SaleId = sale.Id, ProductId = productId, Quantity = quantity, UnitPrice = unitPrice };
        _db.SaleDetails.Add(detail);
        await _db.SaveChangesAsync();

        var url = await _pdf.GenerateReceiptAsync(sale.Id);
        TempData["ReceiptUrl"] = url;
        return RedirectToAction(nameof(Details), new { id = sale.Id });
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var sale = await _db.Sales
            .Include(s => s.Customer)
            .Include(s => s.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return NotFound();
        ViewBag.ReceiptUrl = TempData["ReceiptUrl"]?.ToString();
        return View(sale);
    }
}


