using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;

namespace AdminConstruct.Web.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ExcelImportController : Controller
{
    private readonly ApplicationDbContext _context;

    public ExcelImportController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Admin/ExcelImport/ExcelImport
    public IActionResult ExcelImport()
    {
        return View();
    }

    // POST: /Admin/ExcelImport/ExcelImport
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcelImport(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["ErrorMessage"] = "⚠️ No se seleccionó ningún archivo.";
            return RedirectToAction("ExcelImport");
        }

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var log = new List<string>();
        int insertedProducts = 0;
        int updatedProducts = 0;
        int insertedCustomers = 0;
        int updatedCustomers = 0;

        using var package = new ExcelPackage(new FileInfo(filePath));
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            TempData["ErrorMessage"] = "⚠️ El archivo no contiene hojas.";
            return RedirectToAction("ExcelImport");
        }

        var rowCount = worksheet.Dimension.Rows;

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                string name = worksheet.Cells[row, 1].Text.Trim();
                string priceText = worksheet.Cells[row, 2].Text.Trim();
                string stockText = worksheet.Cells[row, 3].Text.Trim();
                string descOrEmail = worksheet.Cells[row, 4].Text.Trim();
                string document = worksheet.Cells[row, 5].Text.Trim();
                string phone = worksheet.Cells[row, 6].Text.Trim();

                bool hasEmail = descOrEmail.Contains("@");

                if (!hasEmail && !string.IsNullOrEmpty(name))
                {
                    decimal.TryParse(priceText, out decimal price);
                    int.TryParse(stockText, out int stock);

                    if (string.IsNullOrEmpty(name) || price <= 0)
                    {
                        log.Add($"Fila {row}: Producto inválido (nombre o precio incorrecto).");
                        continue;
                    }

                    var existing = _context.Products.FirstOrDefault(p => p.Name == name);

                    if (existing == null)
                    {
                        _context.Products.Add(new Product
                        {
                            Name = name,
                            Price = price,
                            StockQuantity = stock,
                            Description = descOrEmail
                        });
                        insertedProducts++;
                        log.Add($"Fila {row}: Producto '{name}' agregado.");
                    }
                    else
                    {
                        existing.Price = price;
                        existing.StockQuantity = stock;
                        existing.Description = descOrEmail;
                        _context.Products.Update(existing);
                        updatedProducts++;
                        log.Add($"Fila {row}: Producto '{name}' actualizado.");
                    }
                }
                else if (hasEmail && !string.IsNullOrEmpty(name))
                {
                    if (string.IsNullOrEmpty(descOrEmail))
                    {
                        log.Add($"Fila {row}: Cliente sin correo electrónico.");
                        continue;
                    }

                    var existing = _context.Customers.FirstOrDefault(c => c.Email == descOrEmail);

                    if (existing == null)
                    {
                        _context.Customers.Add(new Customer
                        {
                            Name = name,
                            Email = descOrEmail,
                            Document = document,
                            Phone = phone
                        });
                        insertedCustomers++;
                        log.Add($"Fila {row}: Cliente '{name}' agregado.");
                    }
                    else
                    {
                        existing.Name = name;
                        existing.Document = document;
                        existing.Phone = phone;
                        _context.Customers.Update(existing);
                        updatedCustomers++;
                        log.Add($"Fila {row}: Cliente '{name}' actualizado.");
                    }
                }
                else
                {
                    log.Add($"Fila {row}: No se pudo determinar el tipo de registro.");
                }
            }
            catch (Exception ex)
            {
                log.Add($"Fila {row}: Error - {ex.Message}");
            }
        }

        await _context.SaveChangesAsync();

        var logsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/logs");
        if (!Directory.Exists(logsFolder))
            Directory.CreateDirectory(logsFolder);

        var logFile = Path.Combine(logsFolder, $"import_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        await System.IO.File.WriteAllLinesAsync(logFile, log);

        TempData["SuccessMessage"] = $"✅ Importación completada: " +
                                     $"{insertedProducts} productos nuevos, {updatedProducts} actualizados, " +
                                     $"{insertedCustomers} clientes nuevos, {updatedCustomers} actualizados. " +
                                     $"Log guardado en /wwwroot/logs.";

        return RedirectToAction("Index", "Admin");
    }
}
