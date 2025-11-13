using AdminConstruct.Ryzor.ViewModels;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.Web.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 LISTAR (GET: /Products)
    public async Task<IActionResult> Index()
    {
        var productos = await _context.Products.ToListAsync();

        // Mapear a ViewModel
        var model = productos.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Description = p.Description
        }).ToList();

        return View("~/Views/Admin/Products/productos.cshtml", model);
    }

    // 🔹 CREAR (GET)
    public IActionResult Create()
    {
        return View("~/Views/Admin/Products/Create.cshtml", new ProductViewModel());
    }

    // 🔹 CREAR (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);

        try
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Description = model.Description
            };

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al guardar el producto: {ex.Message}");
            return View(model);
        }
    }

    // 🔹 EDITAR (GET)
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Description = product.Description
        };

        return View("~/Views/Admin/Products/Edit.cshtml", model);
    }

    // 🔹 EDITAR (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProductViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = model.Name;
            product.Price = model.Price;
            product.StockQuantity = model.StockQuantity;
            product.Description = model.Description;

            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Products.Any(p => p.Id == id)) 
                return NotFound();
            else 
                throw;
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al actualizar el producto: {ex.Message}");
            return View(model);
        }
    }

    // 🔹 DETALLES
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Description = product.Description
        };

        return View("~/Views/Admin/Products/Details.cshtml", model);
    }

    // 🔹 ELIMINAR (GET)
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Description = product.Description
        };

        return View("~/Views/Admin/Products/Delete.cshtml", model);
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
