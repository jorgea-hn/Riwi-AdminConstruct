using AdminConstruct.Ryzor.Data;
using AdminConstruct.Ryzor.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace AdminConstruct.Ryzor.Controllers;

public class ImportController: Controller
{
     private readonly ApplicationDbContext _context;

    public ImportController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: mostrar formulario de carga
    public IActionResult Upload()
    {
        return View();
    }

    // POST: procesar archivo Excel
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length <= 0)
        {
            ModelState.AddModelError("", "Debes seleccionar un archivo Excel.");
            return View();
        }

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
        {
            ModelState.AddModelError("", "El archivo no contiene hojas.");
            return View();
        }

        var rowCount = worksheet.Dimension.Rows;

        var log = new List<string>();

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                // Ejemplo: detectar tipo de fila según columnas
                var firstCell = worksheet.Cells[row, 1].Text;

                if (decimal.TryParse(worksheet.Cells[row, 3].Text, out var price)) 
                {
                    // Es un producto
                    var product = new Product
                    {
                        Name = worksheet.Cells[row, 2].Text,
                        Price = price,
                        StockQuantity = int.Parse(worksheet.Cells[row, 4].Text),
                        Description = worksheet.Cells[row, 5].Text
                    };
                    _context.Products.Add(product);
                }
                else if (worksheet.Cells[row, 3].Text.Contains("@"))
                {
                    // Es un cliente
                    var customer = new Customer
                    {
                        Name = worksheet.Cells[row, 2].Text,
                        Email = worksheet.Cells[row, 3].Text,
                        Document = worksheet.Cells[row, 4].Text,
                        Phone = worksheet.Cells[row, 5].Text
                    };
                    _context.Customers.Add(customer);
                }
                else
                {
                    log.Add($"Fila {row}: No se pudo identificar el tipo de registro.");
                }
            }
            catch (Exception ex)
            {
                log.Add($"Fila {row}: {ex.Message}");
            }
        }

        await _context.SaveChangesAsync();

        ViewBag.Log = log;
        return View("UploadResult");
    }
}