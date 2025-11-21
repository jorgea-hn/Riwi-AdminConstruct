using AdminConstruct.Web.Data;
using AdminConstruct.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.Web.Controllers;

// [Area("Admin")] // <-- ELIMINADO
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // La vista para esta acción se buscará en /Views/Admin/Index.cshtml
    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalMachinery = await _context.Machineries.CountAsync(),
            TotalCustomers = await _context.Customers.CountAsync(),
            TotalSales = await _context.Sales.CountAsync(),
            TotalRevenue = await _context.Sales.SelectMany(s => s.Details).SumAsync(d => d.UnitPrice * d.Quantity)
        };

        return View(viewModel);
    }
}
