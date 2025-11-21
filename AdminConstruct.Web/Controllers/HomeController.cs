using System.Diagnostics;
using AdminConstruct.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminConstruct.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // *** AÑADIDO: Acción para la página de Acceso Denegado ***
    public IActionResult AccessDenied()
    {
        // Esta acción simplemente muestra la vista AccessDenied.cshtml
        return View(); 
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
