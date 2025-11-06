using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminConstruct.Ryzor.Data;
using AdminConstruct.Ryzor.Models;

namespace AdminConstruct.Ryzor.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    // 🔹 Inyección de dependencias (recibe el contexto)
    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 LISTAR (GET: /Products)
    public async Task<IActionResult> Index()
    {
        var productos = await _context.Products.ToListAsync();
        return View("~/Views/Admin/Products/productos.cshtml", productos);
    }

    // 🔹 CREAR (GET)
    public IActionResult Create()
    {
        return View("~/Views/Admin/Products/Create.cshtml");
    }

    // 🔹 CREAR (POST)
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Create(Product product)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         _context.Add(product);
    //         await _context.SaveChangesAsync();
    //         return RedirectToAction(nameof(Index));
    //     }
    //     return View("~/Views/Admin/Products/Create.cshtml", product);
    // }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(product);

            // Validar que el precio no sea negativo
            if (product.Price < 0)
            {
                ModelState.AddModelError("Price", "El precio no puede ser negativo.");
                return View(product);
            }

            // Validar que el stock sea un número válido
            if (product.StockQuantity < 0)
            {
                ModelState.AddModelError("StockQuantity", "El stock debe ser mayor o igual a 0.");
                return View(product);
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (FormatException)
        {
            ModelState.AddModelError("", "Error de formato: asegúrate de ingresar números válidos.");
            return View(product);
        }
        catch (Exception ex)
        {
            // Mensaje general
            ModelState.AddModelError("", $"Ocurrió un error inesperado: {ex.Message}");
            return View(product);
        }
    }


    // 🔹 EDITAR (GET)
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        return View("~/Views/Admin/Products/Edit.cshtml", product);
    }

    // 🔹 EDITAR (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Product product)
    {
        if (id != product.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View("~/Views/Admin/Products/Edit.cshtml", product);
    }

    // 🔹 DETALLES
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();

        return View("~/Views/Admin/Products/Details.cshtml", product);
    }

    // 🔹 ELIMINAR (GET)
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();

        return View("~/Views/Admin/Products/Delete.cshtml", product);
    }

    // 🔹 ELIMINAR (POST)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
