using Microsoft.AspNetCore.Mvc;

namespace AdminConstruct.Ryzor.Controllers;

public class ProductsController:Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Admin/Products/productos.cshtml");
    }
}