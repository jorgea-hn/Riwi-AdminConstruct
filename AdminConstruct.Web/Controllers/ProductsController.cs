using AdminConstruct.Web.ViewModels;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AdminConstruct.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: /Admin/Products
    public async Task<IActionResult> Index()
    {
        var productos = await _context.Products.ToListAsync();
        var model = productos.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Description = p.Description,
            ImageUrl = p.ImageUrl
        }).ToList();

        return View(model);
    }

    // GET: /Admin/Products/Create
    public IActionResult Create()
    {
        return View(new ProductViewModel());
    }

    // POST: /Admin/Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid) 
            return View(model);

        string? imageUrl = null;
        
        // Procesar imagen si se cargó
        if (ImageFile != null && ImageFile.Length > 0)
        {
            // Validar tamaño (5MB max)
            if (ImageFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("ImageFile", "La imagen no debe superar 5MB");
                return View(model);
            }
            
            // Validar formato
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(ImageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageFile", "Formato no válido. Use JPG, PNG o WEBP");
                return View(model);
            }
            
            // Guardar imagen
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            Directory.CreateDirectory(uploadsFolder);
            
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }
            
            imageUrl = $"/images/products/{uniqueFileName}";
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Price = model.Price,
            StockQuantity = model.StockQuantity,
            Description = model.Description,
            ImageUrl = imageUrl
        };

        _context.Add(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Products/Edit/5
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
            Description = product.Description,
            ImageUrl = product.ImageUrl
        };

        return View(model);
    }

    // POST: /Admin/Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProductViewModel model, IFormFile? ImageFile)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // Procesar nueva imagen si se cargó
        if (ImageFile != null && ImageFile.Length > 0)
        {
            // Validar tamaño (5MB max)
            if (ImageFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("ImageFile", "La imagen no debe superar 5MB");
                return View(model);
            }
            
            // Validar formato
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(ImageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageFile", "Formato no válido. Use JPG, PNG o WEBP");
                return View(model);
            }
            
            // Eliminar imagen anterior si existe
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            
            // Guardar nueva imagen
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            Directory.CreateDirectory(uploadsFolder);
            
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }
            
            product.ImageUrl = $"/images/products/{uniqueFileName}";
        }

        product.Name = model.Name;
        product.Price = model.Price;
        product.StockQuantity = model.StockQuantity;
        product.Description = model.Description;

        _context.Update(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Products/Delete/5
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

        return View(model);
    }

    // POST: /Admin/Products/Delete/5
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
