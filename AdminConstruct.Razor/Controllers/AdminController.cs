using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminConstruct.Razor.Controllers;

[Authorize(Policy = "Administrador")] // Solo acceso a administradores
public class AdminController: Controller
{
   
    
        public IActionResult Index()
        {
            return View(); // Aquí va la vista principal del dashboard
        }

        public IActionResult Productos()
        {
            return View(); // Vista para gestión de productos
        }

        public IActionResult Clientes()
        {
            return View(); // Vista para gestión de clientes
        }

        public IActionResult Ventas()
        {
            return View(); // Vista para gestión de ventas
        }
        
        public IActionResult ExcelImports()
        {
            return View(); // Vista para gestión upload excel
        }
    }
