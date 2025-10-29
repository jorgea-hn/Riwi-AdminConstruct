using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminConstruct.Ryzor.Models;
using AdminConstruct.Ryzor.Data;
using AdminConstruct.Ryzor.Models.ViewModels;

namespace AdminConstruct.Ryzor.Controllers;

[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index()
    {
        var vm = new DashboardViewModel
        {
            TotalProducts = _db.Products.Count(),
            TotalCustomers = _db.Customers.Count(),
            TotalSales = _db.Sales.Count()
        };
        return View(vm);
    }

    [AllowAnonymous]
    public IActionResult Age()
    {
        return View(new AgeViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Age(AgeViewModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.AgeInput))
            {
                ModelState.AddModelError(nameof(model.AgeInput), "La edad es requerida.");
                return View(model);
            }

            model.AgeParsed = int.Parse(model.AgeInput);
            model.Message = $"Edad válida: {model.AgeParsed}";
            return View(model);
        }
        catch (FormatException)
        {
            ModelState.AddModelError(nameof(model.AgeInput), "Por favor ingresa un número entero válido.");
            return View(model);
        }
        catch (OverflowException)
        {
            ModelState.AddModelError(nameof(model.AgeInput), "El número ingresado es demasiado grande.");
            return View(model);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado. Intenta nuevamente.");
            return View(model);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}